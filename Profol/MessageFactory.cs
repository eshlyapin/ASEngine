using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profol
{
    class MessageFactory
    {
        public static Message CreateMessage(MessageHeader header)
        {
            switch (header.PacketType)
            {
                case 1: // login
                    return new MessageLogin(header);
                default:
                    return new Message(null);
            }
        }
    }
}
