using Game_Core.Blocks;
using Game_Core.Messages.OutgoingMessages;
using Game_Core.Utilities;

namespace Game_Core.Obstacles
{
    public class Obstacle : LivingObject
    {
        private Arena arena;
        public Vector3 position;
        public float radius;
        public float maxHealth;
        public float health;
        public int skinId;

        public bool IsAlive
        {
            get { return health > 0; }
        }

        public Obstacle() { }

        public Obstacle(Arena arena, Vector3 position, float health=40f, float radius=0.15f, int skinId=0)
        {
            this.arena = arena;
            this.position = position;
            this.health = this.maxHealth = health;
            this.radius = radius;
            this.skinId = skinId;
        }

        public override void Update()
        {
            if (awaitsDestruction)
                return;

            if (health <= 0)
                awaitsDestruction = true;
        }

        public void DealDamage(float damage)
        {
            if (health <= 0 || damage < health)
                health -= damage;
            else
            {
                health -= damage;
                OM_ObstacleHealthInfo.SendMessage(arena,this, new Vector3(0,0,1));
            }
        }

        public virtual bool CollidesWith(Vector3 point, float pointRadius)
        {
            return (position - point).magnitude < radius + pointRadius;
        }
    }
}
