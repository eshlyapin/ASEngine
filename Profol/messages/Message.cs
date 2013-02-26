using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Profol
{
    public abstract class Message
    {
        public MessageHeader Header { get; protected set; }
        public abstract byte[] ToBytes();
    }
}
