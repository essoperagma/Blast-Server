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
    class OM_ShieldSkillUsed : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena, Player skillUser)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_ShieldSkillUsed));

            message.putInt("playerId",skillUser.id);

            foreach(var p in arena.players)
                p.user.client.SendMessage(message);
        }
    }
}
