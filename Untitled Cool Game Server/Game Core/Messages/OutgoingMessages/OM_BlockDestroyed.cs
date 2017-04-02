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
    class OM_BlockDestroyed : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena, int blockIndexI, int blockIndexJ)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_BlockDestroyed));

            message.putInt("indexI",blockIndexI);
            message.putInt("indexJ",blockIndexJ);

            foreach(var p in arena.players)
                p.user.client.SendMessage(message);
        }
    }
}
