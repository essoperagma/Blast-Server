using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetWorker.EventArgs;
using NetWorker.Utilities;

namespace GameCore.Messages
{
    public interface IIncomingMessage
    {
        void processMessage(MessageArrivedEventArgs messageArgs);
    }
}
