using System;
using System.Collections.Generic;
using System.Linq;
using Game_Core.Blocks;
using Game_Core.Messages.OutgoingMessages;
using Game_Core.Skills;
using Game_Core.Utilities;
using NetWorker.Host;

namespace Game_Core
{
    public class Player : LivingObject
    {
        public Vector3 position;
        public float health;
        public float maxHealth;
        public bool IsAlive { get { return health > 0; } }
        public float radius;
        public Vector3 walkTarget;
        public Vector3 flyTarget;
        public float walkSpeed;
        public float flySpeed;
        public float walkSpeedCoefficient=1f;
        public User user;
        private float score;
        public Arena arena;
        public int gold;

        public PlayerEffects effects;

        public enum PlayerState { AnsweringReady, WaitingOthersReady, Loading, WaitingOthersLoading, InGame}
        public PlayerState state { get; set; }
        
        private enum MovementState { Idle, Walking, Flying}
        private MovementState movementState;

        public readonly List<Skill> skills = new List<Skill>();  // skill'ler kullanici actikca sirayla ekleniyor. index based cagiriliyor.

        #region LAST HIT
        private const int LAST_HIT_GOLD = 3;
        private const float LAST_HIT_EXPIRE_TIME=15;
        private float remainingLastHitTime;
        public Player lastHitStamp;
        #endregion

        public Player(User user, Arena arena)
        {
            this.user = user;
            this.arena = arena;

            PrepareForRound();

            skills.Add(new FireballSkill(this));
            skills.Add(new TeleportSkill(this));
            //skills.Add(new ShieldSkill(this));
        }

        public void PrepareForRound()
        {
            movementState = MovementState.Idle;
            health = 100;
            maxHealth = 100;
            radius = .25f;
            walkSpeed = 2.25f;
            flySpeed = 3f;   
        }

        public override void Update()
        {
            #region Walk and Fly Stuff
            if ( (IsAlive || movementState == MovementState.Flying) && state == PlayerState.InGame)
            {
                Vector3 prePosition = position;

                if (movementState == MovementState.Flying && Move(flyTarget, flySpeed))
                    movementState = MovementState.Idle;
                else if (movementState == MovementState.Walking && Move(walkTarget, walkSpeed*walkSpeedCoefficient))
                    movementState = MovementState.Idle;

                Vector3 postPosition = position;

                if (prePosition != postPosition && (arena.obstacles.Any(o => o.CollidesWith(postPosition, radius)) || // eger bir obstacle a carpmissam
                    arena.players.Any(o => o != this && o.IsAlive && o.CollidesWith(postPosition, radius))))// eger bir player a carpmissam
                {
                    position = prePosition; // movement hamlesini geri alalim.
                    movementState = MovementState.Idle; // yurumeyi durduralim.
                }
            }
            #endregion

            foreach (var skill in skills)
                skill.Update();

            effects.Update();

            CheckLastHitStampExpire();
        }

        private bool Move(Vector3 to, float speed)
        {
            float deltaDistance = speed*Chronos.deltaTime;
            float distance = (position - to).magnitude;

            if (distance < deltaDistance)
            {
                position = to;
                return true;
            }

            position += (to - position).normalized*deltaDistance;
            return false;
        }

        public void WalkRequest(Vector3 to)
        {
            if (movementState != MovementState.Flying)
            {
                movementState = MovementState.Walking;
                walkTarget = to;
                
                OM_PlayerWalk.SendMessage(arena,this);
            }
        }

        public void FlyRequest(Vector3 to)
        {
            movementState = MovementState.Flying;
            flyTarget = to;

            OM_PlayerFly.SendMessage(arena, this);
        }

        public virtual bool CollidesWith(Vector3 point, float pointRadius)
        {
            return (position - point).magnitude < radius + pointRadius;
        }

        public void DealDamage(float damage, Player lastHitStamp = null)
        {
            AssignLastHitStamp(lastHitStamp);

            if (health <= 0 || damage < health)
                health -= damage;
            else
            {
                health -= damage;

                // can ilk kez sifirin altina dusuyor.
                // mesajin ulastigina emin olalim, clientta 'NoKill' damage verenler sorun cikarmasin.
                OM_PlayerHealthInfo.SendMessage(this.arena,this);
                arena.roundStats.PlayerDied(this);
                OnDied();
            }
        }

        private void OnDied()
        {
            if (lastHitStamp != null)
            {
                arena.roundStats.PlayerKilledOpponent(lastHitStamp,this);
                lastHitStamp.UpdateGold( lastHitStamp.gold+LAST_HIT_GOLD);
                OM_PlayerGoldInfo.SendMessage(lastHitStamp);
                lastHitStamp = null;
            }
        }

        public bool SpendGold(int amount)
        {
            if (gold < amount)
                return false;
            gold -= amount;
            return true;
        }

        public void UpdateGold(int value)
        {
            this.gold = value;
        }

        public void TeleportTo(Vector3 position)
        {
            movementState = MovementState.Idle;
            this.position = position;
        }

        public void Destroy()
        {
            awaitsDestruction = true;
        }

        private void AssignLastHitStamp(Player byPlayer)
        {
            if(byPlayer != null)
            {
                remainingLastHitTime = LAST_HIT_EXPIRE_TIME;
                lastHitStamp = byPlayer;
            }
        }

        private void CheckLastHitStampExpire()
        {
            if (remainingLastHitTime > 0)
                remainingLastHitTime -= Chronos.deltaTime;
            if (remainingLastHitTime <= 0)
                lastHitStamp = null;
        }
    }

    public struct PlayerEffects
    {
        public bool isStunned;
        public float stunDuration;

        public bool hasShield;
        public float shieldDuration;

        public void Update()
        {
            if (isStunned)
            {
                stunDuration -= Chronos.deltaTime;
                if (stunDuration <= 0)
                    isStunned = false;
            }

            if (hasShield)
            {
                shieldDuration -= Chronos.deltaTime;
                if (shieldDuration <= 0)
                    hasShield = false;
            }
        }
    }
}
