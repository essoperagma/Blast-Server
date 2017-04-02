using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core.Messages.OutgoingMessages;
using Game_Core.Utilities;

namespace Game_Core.Skills
{
    class ShieldSkill : Skill
    {
        private const float COOLDOWN = 7;
        private const float SKILL_DURATION = 3f;

        private float remainingCooldown;

        public ShieldSkill() { }

        public ShieldSkill(Player player)
        {
            this.player = player;
        }

        public override void Update()
        {
            if (remainingCooldown > 0)
                remainingCooldown -= Chronos.deltaTime;
        }

        public override bool IsActivatable(Vector3 activationLocation)
        {
            return remainingCooldown <= 0;
        }

        public override void ActivateAndNotify(Vector3 activationLocation)
        {
            player.effects.hasShield = true;
            player.effects.shieldDuration = SKILL_DURATION;
            OM_ShieldSkillUsed.SendMessage(player.arena, player);

            remainingCooldown = COOLDOWN;
        }
    }
}
