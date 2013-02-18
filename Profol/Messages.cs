using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Profol
{
    class MessageHeader
    {
        public byte PacketType { get; private set; }
        public uint PacketSize { get; private set; }

        byte[] _buffer;

        public MessageHeader(byte[] buffer)
        {
            _buffer = buffer;
            PacketType = _buffer[0];
            PacketSize = BitConverter.ToUInt32(_buffer, 1);
        }

        public override string ToString()
        {
            return "Type: " + PacketType + " Size: " + PacketSize;
        }
    }

    class Message
    {
        protected MessageHeader _header;
        protected byte[] _buffer;
        public Message(MessageHeader header)
        {
            _header = header;
            _buffer = new byte[header.PacketSize];
        }

        public override string ToString()
        {
            return "MESSAGE: " + _header.ToString();
        }
    }

    class MessageLogin : Message
    {
        public string Name {get; protected set;}
        public string Password { get; protected set; }

        public MessageLogin(MessageHeader header)
            :base(header)
        {
            Name = ReadName();
            Password = ReadPassword();
        }

        string ReadName()
        {
            return Encoding.ASCII.GetString(_buffer);
        }

        string ReadPassword()
        {
            int i = 0;
            for (i = 0; i < _buffer.Length; ++i)
            {
                if (_buffer[i] == 0x0)
                    break;
            }
            return Encoding.ASCII.GetString(_buffer, i, _buffer.Length - i);
        }

        public override string ToString()
        {
            return base.ToString() + " LOGIN: " + Name + " PASSWORD: " + Password;
        }
        
    }
}
