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
    class OM_StartCountdown : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena, float duration)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_StartCountdown));
            
            message.putFloat("duration", duration);
            message.putInt("round", arena.currentRound);
            message.putInt("totalRounds", arena.totalRounds);

            foreach(var p in arena.players)
                p.user.client.SendMessage(message);
        }
    }
}
