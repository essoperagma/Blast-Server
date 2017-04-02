using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Blocks;
using Game_Core.Missiles;
using Game_Core.Utilities;
using NetWorker.Utilities;

namespace Game_Core.Messages.OutgoingMessages
{
    class OM_MissileExplode : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena, Missile missile)
        {
            RawMessage message = PrepareMessageFor(typeof(OM_MissileExplode));

            message.putInt("missileId", missile.id);
            message.PutVector3("explosionPosition", missile.position);

            foreach (var player in arena.players)
                player.user.client.SendMessage(message);
        }
    }
}
