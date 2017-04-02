using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Messages;
using NetWorker.Host;
using NetWorker.Utilities;

namespace Game_Core.Messages.OutgoingMessages
{
    class OM_LeaveQueueResult : OutgoingMessageImp
    {
        public static void SendMessage(Client client, bool success)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_LeaveQueueResult));
            message.putBool("success", success);
            client.SendMessage(message);
        }
    }
}
