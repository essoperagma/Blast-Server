using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Blocks;
using Game_Core.Utilities;
using NetWorker.EventArgs;

namespace Game_Core.Messages.IncomingMessages
{
    class IM_PlayerWalkRequest : IncomingMessageImp
    {
        public override void processMessage(MessageArrivedEventArgs messageArgs)
        {
            Player player;
            if (Game.clientPlayerMap.TryGetValue(messageArgs.sender, out player))
            {
                if (player.state == Player.PlayerState.InGame && player.arena.state == Arena.ArenaState.Battle && player.IsAlive)
                {
                    Vector3 walkTarget = messageArgs.message.GetVector3("walkTarget");
                    walkTarget.y = 0;
                    player.WalkRequest(walkTarget);
                }
            }
        }
    }
}
