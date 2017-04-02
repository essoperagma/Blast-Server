using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Blocks;
using NetWorker.EventArgs;

namespace Game_Core.Messages.IncomingMessages
{
    class IM_ExitGame : IncomingMessageImp
    {
        public override void processMessage(MessageArrivedEventArgs messageArgs)
        {
            Player player;
            
            if (Game.clientPlayerMap.TryGetValue(messageArgs.sender, out player))
            {
                //if (player.arena.state == Arena.ArenaState.Battle || player.arena.state == Arena.ArenaState.RoundOver || player.arena.state == Arena.ArenaState.BattleOver)
                    player.arena.PlayerKick(player);
            }
        }
    }
}
