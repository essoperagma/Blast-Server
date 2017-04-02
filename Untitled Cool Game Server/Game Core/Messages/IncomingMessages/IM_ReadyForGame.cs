using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using NetWorker.EventArgs;

namespace Game_Core.Messages.IncomingMessages
{
    class IM_ReadyForGame : IncomingMessageImp
    {
        public override void processMessage(MessageArrivedEventArgs messageArgs)
        {
            Player player;

            if (Game.clientPlayerMap.TryGetValue(messageArgs.sender, out player) && player.state == Player.PlayerState.AnsweringReady)
            {
                bool ready = messageArgs.message.getBool("isReady");

                if (ready)
                {
                    player.state = Player.PlayerState.WaitingOthersReady;
                    player.arena.PlayerAcceptedReady(player);
                }
                else
                {
                    player.arena.PlayerRejectedReady(player);
                }
            }
        }
    }
}
