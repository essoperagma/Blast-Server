using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core.Messages.OutgoingMessages;
using Game_Core.Missiles;
using Game_Core.Utilities;

namespace Game_Core.Skills
{
    public class FireballSkill : Skill
    {
        private const float COOLDOWN = 1;
        private float remainingCooldown;

        public FireballSkill() { }

        public FireballSkill(Player player)
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
            Vector3 missileDirection = (activationLocation - player.position). normalized;
            FireballMissile missile = new FireballMissile(player, player.position, missileDirection );
            player.arena.SpawnMissile(missile);
            OM_FireballSkillUsed.SendMessage(player.arena,player,missile);
            
            remainingCooldown = COOLDOWN;
        }
    }
}
