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
    class OM_PlayerFly : OutgoingMessageImp
    {
        public static void SendMessage( Arena arena, Player player)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_PlayerFly));

            message.putInt("playerId", player.id);
            message.putFloat("flySpeed", player.flySpeed);
            message.PutVector3("flyTarget",player.flyTarget);

            foreach(var p in arena.players)
                p.user.client.SendMessage(message);
        }
    }
}
