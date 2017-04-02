using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Messages.OutgoingMessages;
using NetWorker.EventArgs;

namespace Game_Core.Messages.IncomingMessages
{
    class IM_LeaveQueue : IncomingMessageImp
    {
        public override void processMessage(MessageArrivedEventArgs messageArgs)
        {
            User user;
            // login olmus mu?
            if (Game.clientUserMap.TryGetValue(messageArgs.sender, out user))
            {
                if(Game.queue.RemoveUser(user))
                    OM_LeaveQueueResult.SendMessage(messageArgs.sender,true);
            }
        }
    }
}
