using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetWorker.EventArgs;
using NetWorker.Utilities;

namespace GameCore.Messages
{
    abstract class IncomingMessageImp : IIncomingMessage
    {
        public abstract void processMessage(MessageArrivedEventArgs messageArgs);
    }
}
