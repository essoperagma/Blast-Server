using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game_Core.Blocks;
using Game_Core.Obstacles;
using Game_Core.Utilities;

namespace Game_Core.Missiles
{
    public abstract class Missile : LivingObject
    {
        public Vector3 position;
        public float radius=0.20f;
        public float damage=10;
        public float explosionRadius=0.5f;
        public float movementSpeed=6f;
        public Player senderPlayer;
        public Arena arena{ get { return senderPlayer.arena; }}
        public Vector3 movementDirection;
        public float remainingLifetime=2;

        protected bool ignoringSender = true;

        public bool ContainsPoint(Vector3 point)
        {
            return (point - position).magnitude <= radius;
        }

        public virtual bool CollidesWith(Vector3 point, float pointRadius)
        {
            return (position - point).magnitude < radius + pointRadius;
        }

        public override void Update()
        {
            if (awaitsDestruction)
                return;

            if (remainingLifetime >= 0)
            {
                remainingLifetime -= Chronos.deltaTime;

                if (remainingLifetime <= 0)
                    awaitsDestruction = true;

                Walk();
                CheckForSenderSeperation();
                CheckForCollisionContact();
            }


        }

        protected void Walk()
        {
            position += movementSpeed*Chronos.deltaTime*movementDirection;
        }

        /// <summary>
        /// Sender player missile'i gonderir gondermez etki alaninin icinde oluyor ve kendini vuruyor. kendini vurmasina engel olmak icin
        /// missile uretildigi andan itibaren player'la olan collision'u bitene kadar player'i gormezden geliyor.
        /// </summary>
        private void CheckForSenderSeperation()
        {
            if (ignoringSender && senderPlayer.CollidesWith(position, radius) == false)
                ignoringSender = false;
        }
        
        private void CheckForCollisionContact()
        {
            Debug.Assert(arena != null);

            List<Player> contactedPlayers = new List<Player>(arena.players.Count);
            List<Obstacle> contactedObstacles= new List<Obstacle>(arena.obstacles.Count);

            foreach (var p in arena.players.Where(i => i.IsAlive && (ignoringSender == false || i != senderPlayer)))
            {
                if (p.CollidesWith(position, radius))
                {
                    contactedPlayers.AddRange(arena.players.Where(i => i.IsAlive && i.CollidesWith(position, explosionRadius)));
                    contactedObstacles.AddRange(arena.obstacles.Where( i=> i.IsAlive && i.CollidesWith(position, radius)));
                    OnContact(contactedPlayers, contactedObstacles);

                    return;
                }
            }

            foreach (var m in arena.missiles.Where(i => i != this))
            {
                if (m.CollidesWith(position, radius))
                {
                    contactedPlayers.AddRange(arena.players.Where(i => i.IsAlive && i.CollidesWith(position, explosionRadius)));
                    contactedObstacles.AddRange(arena.obstacles.Where(i => i.IsAlive && i.CollidesWith(position, radius)));
                    OnContact(contactedPlayers, contactedObstacles);

                    // 2. missile'e de onplayercontact yaptik. sakincali olabilir, cunku o misilin kendi update dongusune birakmak gerek aslinda bu isi.
                    // ama suan burada yapmazsak, bizim missile awaitsDestruction=true oluyor. diger misile sira geldiginde coktan yokedilmis oldugu icin diger misil patlamadan yoluna devam ediyor.
                    contactedPlayers.Clear();
                    contactedObstacles.Clear();
                    contactedPlayers.AddRange(arena.players.Where(i => i.IsAlive && i.CollidesWith(m.position, m.explosionRadius)));
                    contactedObstacles.AddRange(arena.obstacles.Where(i => i.IsAlive && i.CollidesWith(m.position, m.radius)));
                    m.OnContact(contactedPlayers, contactedObstacles);
                    return;
                }
            }

            foreach (var o in arena.obstacles.Where(obs=>obs.IsAlive))
            {
                if (o.CollidesWith(position, radius))
                {
                    contactedPlayers.AddRange(arena.players.Where(i => i.IsAlive && i.CollidesWith(position, explosionRadius)));
                    contactedObstacles.AddRange(arena.obstacles.Where(i => i.IsAlive && i.CollidesWith(position, radius)));

                    OnContact(contactedPlayers, contactedObstacles);
                    return;
                }
            }
        }

        protected abstract void OnContact(List<Player> players, List<Obstacle> obstacles);

    }
}
