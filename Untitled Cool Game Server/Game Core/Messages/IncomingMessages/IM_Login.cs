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
    class IM_Login : IncomingMessageImp
    {
        public override void processMessage(MessageArrivedEventArgs messageArgs)
        {
            User user;
            // zaten login olmus biri tekrar login olmaya calismasin sakin
            if (Game.clientUserMap.TryGetValue(messageArgs.sender, out user) == false)
            {
                RawMessage message = messageArgs.message;

                if (message != null && message.containsField("username"))
                {
                    string username = message.getUTF8String("username");

                    if (username.StartsWith("ArenaSize:"))
                    {
                        int size = int.Parse(username.Split(':')[1]);
                        Game.queue.maxUserCount = Math.Max(2, size);

                        OM_LoginResult.SendMessage(messageArgs.sender, false);
                    }
                    else
                    {
                        user = new User(messageArgs.sender, username);
                        Game.clientUserMap.Add(messageArgs.sender, user);

                        OM_LoginResult.SendMessage(messageArgs.sender, true, user);
                    }
                }
            }
        }
    }
}
