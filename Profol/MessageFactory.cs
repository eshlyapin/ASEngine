﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profol
{
    class MessageFactory
    {
        public static Message CreateMessage(MessageHeader header, byte[] buffer)
        {
            switch (header.PacketType)
            {
                case 1: // login
                    return new MessageLogin(header, buffer);
                default:
                    throw new ArgumentException("Illegal message type");
                    return null;
            }
        }
    }
}
