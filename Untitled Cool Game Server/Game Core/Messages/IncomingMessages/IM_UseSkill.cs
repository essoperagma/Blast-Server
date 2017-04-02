using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Blocks;
using Game_Core.Skills;
using Game_Core.Utilities;
using NetWorker.EventArgs;

namespace Game_Core.Messages.IncomingMessages
{
    class IM_UseSkill: IncomingMessageImp
    {
        public override void processMessage(MessageArrivedEventArgs messageArgs)
        {
            Player player;

            if (Game.clientPlayerMap.TryGetValue(messageArgs.sender, out player) && player.state == Player.PlayerState.InGame && player.arena.state == Arena.ArenaState.Battle)
            {
                int skillIndex = messageArgs.message.getInt("skillIndex");

                if (player.skills != null && player.skills.Count > skillIndex && player.IsAlive) //dead man cant shoot
                {
                    Skill skill = player.skills[skillIndex];
                    Vector3 usePosition = messageArgs.message.GetVector3("usePosition");
                    if (skill != null && skill.IsActivatable(usePosition))
                        skill.ActivateAndNotify(usePosition);
                }
            }
        }
    }
}
