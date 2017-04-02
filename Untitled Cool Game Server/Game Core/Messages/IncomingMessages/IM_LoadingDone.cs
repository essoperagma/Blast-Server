using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using NetWorker.EventArgs;

namespace Game_Core.Messages.IncomingMessages
{
    class IM_LoadingDone : IncomingMessageImp
    {
        public override void processMessage(MessageArrivedEventArgs messageArgs)
        {
            Player player = null;

            if (Game.clientPlayerMap.TryGetValue(messageArgs.sender, out player))
            {
                player.arena.PlayerDoneLoading(player);
            }
        }
    }
}
