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
    class OM_PlayerHealthInfo : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena, Player p)
        {
            RawMessage message = PrepareMessageFor(typeof(OM_PlayerHealthInfo));

            message.putInt("playerId", p.id);
            message.putFloat("health", p.health);
            
            foreach (var player in arena.players)
                player.user.client.SendMessage(message);
        }
    }
}
