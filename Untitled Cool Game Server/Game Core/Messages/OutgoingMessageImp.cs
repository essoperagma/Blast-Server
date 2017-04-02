using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Core;
using NetWorker.Utilities;

namespace GameCore.Messages
{
    class OutgoingMessageImp : IOutgoingMessage
    {
        protected static RawMessage PrepareMessageFor( Type messageType)
        {
            RawMessage message = new RawMessage();
            message.putInt("messageTypeId", TypeIdGenerator.outgoingMessageIds[ messageType ] );
            return message;
        }
    }
}
