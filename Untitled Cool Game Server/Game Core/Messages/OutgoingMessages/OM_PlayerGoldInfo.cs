using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using NetWorker.Utilities;

namespace Game_Core.Messages.OutgoingMessages
{
    class OM_PlayerGoldInfo : OutgoingMessageImp
    {
        public static void SendMessage(Player player)
        {
            RawMessage message = PrepareMessageFor(typeof(OM_PlayerGoldInfo));

            message.putInt("gold", player.gold);
            
            player.user.client.SendMessage(message);
        }
    }
}
