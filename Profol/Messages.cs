using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Profol
{
    class MessageHeader
    {
        public byte PacketType { get; private set; }
        public uint PacketSize { get; private set; }
        public const uint HeaderSize = sizeof(byte) + sizeof(uint);
        public const uint MAX_PACKET_SIZE = 4096;

        byte[] _buffer;

        public MessageHeader(byte[] buffer)
        {
            _buffer = buffer;

            PacketType = _buffer[0];
            PacketSize = BitConverter.ToUInt32(_buffer, 1);
            if(PacketSize > MAX_PACKET_SIZE)
                throw new InvalidDataException("Size of packet more of:" + MAX_PACKET_SIZE);
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
        public Message(MessageHeader header, byte[] buffer)
        {
            _header = header;
            _buffer = buffer;
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

        public MessageLogin(MessageHeader header, byte[] buffer)
            :base(header, buffer)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(_buffer));
            Name = reader.ReadString();
            Password = reader.ReadString();
        }

        public override string ToString()
        {
            return base.ToString() + " LOGIN: " + Name + " PASSWORD: " + Password;
        }
        
    }
}
