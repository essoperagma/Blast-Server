using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core.Messages.OutgoingMessages;
using Game_Core.Obstacles;
using Game_Core.Utilities;

namespace Game_Core.Missiles
{
    class FireballMissile : Missile
    {
        public FireballMissile() { }

        public FireballMissile(Player sender, Vector3 position, Vector3 direction)
        {
            this.senderPlayer = sender;
            this.position = position;
            this.movementDirection = direction;
        }

        protected override void OnContact(List<Player> players, List<Obstacle> obstacles)
        {
            int goldMade = 0;
            float damageAmount = damage*(1+ RandomInstance.instance.Next(21)/100f-0.1f);  // random +- %10

            foreach(var p in players)
            {
                p.FlyRequest(p.position + ((p.position - position).normalized + movementDirection.normalized).normalized * 3); // ucus mesafesi 3 birim
                
                if (!p.effects.hasShield)
                {
                    p.DealDamage(damageAmount,senderPlayer);
                    OM_PlayerHealthInfo.SendMessage(senderPlayer.arena, p);

                    if(p!=senderPlayer)
                        goldMade++;
                }
                else
                {
                    p.effects.hasShield = false;
                    OM_ShieldConsumed.SendMessage(p.arena, p);
                    p.UpdateGold(p.gold+1);
                    OM_PlayerGoldInfo.SendMessage(p);
                }
            }

            foreach (var o in obstacles)
            {
                o.DealDamage(damageAmount);
                OM_ObstacleHealthInfo.SendMessage(arena,o,position);
            }

            OM_MissileExplode.SendMessage(senderPlayer.arena,this);
            awaitsDestruction = true;

            if (goldMade != 0 && senderPlayer!=null)
            {
                arena.roundStats.PlayerHit(senderPlayer);
                senderPlayer.UpdateGold( senderPlayer.gold + goldMade);
                OM_PlayerGoldInfo.SendMessage(senderPlayer);
            }
        }
    }
}
