using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Blocks;
using NetWorker.Utilities;

namespace Game_Core.Messages.OutgoingMessages
{
    class OM_ReturnToQueue : OutgoingMessageImp
    {
        public static void SendMessage(List<Player> players, Player except )
        {
            RawMessage message = PrepareMessageFor(typeof (OM_ReturnToQueue));

            foreach (var p in players)
            {
                if(p!=except)
                    p.user.client.SendMessage(message);
            }
        }
    }
}
