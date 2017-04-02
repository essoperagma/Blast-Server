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
    class OM_CreateMissile : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena, Missile missile)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_CreateMissile));

            message.putInt("missileTypeId", TypeIdGenerator.idsOfMissiles[missile.GetType()]);

            message.putInt("missileId",missile.id);
            message.PutVector3("position", missile.position);
            message.PutVector3("direction",missile.movementDirection);

            foreach (var player in arena.players)
                player.user.client.SendMessage(message);
        }
    }
}
