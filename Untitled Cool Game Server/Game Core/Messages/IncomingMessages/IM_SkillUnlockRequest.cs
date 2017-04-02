using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Messages.OutgoingMessages;
using Game_Core.Skills;
using NetWorker.EventArgs;

namespace Game_Core.Messages.IncomingMessages
{
    class IM_SkillUnlockRequest : IncomingMessageImp
    {
        public override void processMessage(MessageArrivedEventArgs messageArgs)
        {
            int typeId = messageArgs.message.getInt("skillTypeId");
            int slotIndex = messageArgs.message.getInt("slotIndex");

            Player player;

            if (Game.clientPlayerMap.TryGetValue(messageArgs.sender, out player))
            {
                Type skillType = TypeIdGenerator.skillTypesByIds[typeId].GetType();

                if (player.skills.Any(s => s.GetType() == skillType))
                    return;

                Skill skill = Activator.CreateInstance(skillType, player) as Skill;

                int price = skill.UnlockPrice + player.skills.Count*2;
                if (skill != null && player.SpendGold(price))
                {
                    player.skills.Add(skill);
                    OM_SkillUnlocked.SendMessage(player, typeId, slotIndex);
                    OM_PlayerGoldInfo.SendMessage(player);
                }
            }
        }
    }
}
