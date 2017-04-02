using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using Game_Core.Messages.OutgoingMessages;
using NetWorker.EventArgs;
using NetWorker.Utilities;

namespace Game_Core.Messages.IncomingMessages
{
    class IM_JoinQueue : IncomingMessageImp
    {
        public override void processMessage(MessageArrivedEventArgs messageArgs)
        {
            User user;
            // login olmamis biri join olamamali
            if (Game.clientUserMap.TryGetValue(messageArgs.sender, out user))
            {
                OM_JoinQueueResult.SendMessage(messageArgs.sender, true);
                Game.queue.AddUser(user);   // bu metodun icinde kullanici var mi diye kontrol ediyoruz, varsa eklemiyoruz. 
            }
        }
    }
}
