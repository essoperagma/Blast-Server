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
    class OM_FireballSkillUsed : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena, Player skillUser, FireballMissile spawned)
        {
            RawMessage message = PrepareMessageFor(typeof(OM_FireballSkillUsed));

            message.putInt("playerId", skillUser.id);
            message.putInt("missileId", spawned.id);
            message.PutVector3("position", spawned.position);
            message.PutVector3("direction", spawned.movementDirection);

            foreach (var player in arena.players)
                player.user.client.SendMessage(message);
        }
    }
}
