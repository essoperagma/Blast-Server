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
    class OM_BattleOver : OutgoingMessageImp
    {
        public static void SendMessage(Arena arena)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_BattleOver));
            
            // TODO kazanan oyuncu
            // harcamalar, puanlar, kim ne kazandi, oyun ne kadar surdu. ne gondereceksen artik burada gonder.
            
            arena.players.ForEach( (p)=>p.user.client.SendMessage(message));
        }
    }
}
