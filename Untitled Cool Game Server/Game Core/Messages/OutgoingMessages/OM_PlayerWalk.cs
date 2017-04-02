using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Blocks;
using Game_Core.Utilities;
using NetWorker.Utilities;

namespace Game_Core.Messages.OutgoingMessages
{
    class OM_PlayerWalk : OutgoingMessageImp
    {
        public static void SendMessage( Arena arena, Player player)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_PlayerWalk));

            message.putInt("playerId", player.id);
            message.putFloat("walkSpeed", player.walkSpeed);
            message.PutVector3("walkTarget",player.walkTarget);
            message.PutVector3("currentPosition", player.position);

            foreach(var p in arena.players)
                p.user.client.SendMessage(message);
        }
    }
}
