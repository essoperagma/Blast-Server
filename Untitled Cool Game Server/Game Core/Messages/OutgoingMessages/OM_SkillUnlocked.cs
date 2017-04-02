using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Blocks;
using Game_Core.Skills;
using NetWorker.Utilities;

namespace Game_Core.Messages.OutgoingMessages
{
    class OM_SkillUnlocked : OutgoingMessageImp
    {
        public static void SendMessage(Player player, int skillTypeId, int skillSlotIndex)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_SkillUnlocked));
            
            message.putInt( "skillTypeId", skillTypeId);
            message.putInt("slotIndex",skillSlotIndex);

            player.user.client.SendMessage(message);
        }
    }
}
