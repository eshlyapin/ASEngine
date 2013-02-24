using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profol
{
    public class MessageFactory
    {
        public static Message CreateMessage(MessageHeader header, byte[] buffer)
        {
            switch (header.PacketType)
            {
                case 1: // login
                    return new LoginMessage(header, buffer);
                default:
                    throw new ArgumentException("Illegal message type");
            }
        }

        public static LoginMessage CreateLoginMessage(string username, string password)
        {
            return new LoginMessage(username, password);
        }
    }
}
