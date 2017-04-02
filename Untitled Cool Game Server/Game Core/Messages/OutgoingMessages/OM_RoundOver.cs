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
    class OM_RoundOver : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_RoundOver));

            int[] deathOrder =new int[arena.roundStats.deathOrder.Count];
            int[] finalScores = arena.roundStats.finalGolds;

            List<Player> deathOrderList = arena.roundStats.deathOrder;
            for (int i = 0; i < arena.roundStats.deathOrder.Count; i++)
            {
                deathOrder[i] = deathOrderList[i].id;
            }

            message.putInt("winnerCount",arena.roundStats.winnerCount);
            message.putIntArray("deathOrder", deathOrder);
            message.putIntArray("finals", finalScores);
            message.putIntArray("kills", arena.roundStats.kills);
            message.putIntArray("hits", arena.roundStats.hits);

            foreach (var p in arena.players)
                p.user.client.SendMessage(message);
        }
    }
}
