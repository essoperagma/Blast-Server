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
    class OM_LoginResult : OutgoingMessageImp
    {
        public static void SendMessage(Client client, bool success, User user = null)
        {
            RawMessage message = PrepareMessageFor(typeof (OM_LoginResult));
            message.putBool("success", success);

            if (success)
            {
                message.putUTF8String("username", user.username);
                message.putInt("id",user.id);
            }

            client.SendMessage(message);
        }
    }
}
