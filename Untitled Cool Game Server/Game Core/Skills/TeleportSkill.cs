using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core.Messages.OutgoingMessages;
using Game_Core.Utilities;

namespace Game_Core.Skills
{
    class TeleportSkill : Skill
    {
        private const float MAX_TP_DISTANCE = 7;
        private const float COOLDOWN = 5;
        private float remainingCooldown;

        public TeleportSkill() { }

        public TeleportSkill(Player player)
        {
            this.player = player;
        }

        public override void Update()
        {
            if(remainingCooldown > 0)
                remainingCooldown -= Chronos.deltaTime;
        }

        public override bool IsActivatable(Vector3 activationLocation)
        {
            return remainingCooldown <= 0;
        }

        public override void ActivateAndNotify(Vector3 activationLocation)
        {
            float distance = (activationLocation - player.position).magnitude;
            distance = Math.Min(distance, MAX_TP_DISTANCE);

            Vector3 tpTarget= player.position + (activationLocation - player.position). normalized*distance;

            if( player.arena.players.Any(p=> p!= player && p.CollidesWith(tpTarget,player.radius)) == false &&      // eger teleport olmak istedigi yerde baska obje yoksa.
                player.arena.obstacles.Any(o=> o.CollidesWith(tpTarget,player.radius)) == false)
            {
                player.TeleportTo(tpTarget);
                OM_TeleportSkillUsed.SendMessage(player.arena, player);

                remainingCooldown = COOLDOWN;
            }
        }
    }
}
