using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Profol
{
    public class MessageHeader
    {
        public byte PacketType { get; private set; }
        public uint PacketSize { get; private set; }
        public const uint HeaderSize = sizeof(byte) + sizeof(uint);
        public const uint MAX_PACKET_SIZE = 4096;

        byte[] mBuffer;

        public MessageHeader(byte[] buffer)
        {
            mBuffer = buffer;
            using (MemoryStream stream = new MemoryStream(buffer))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                PacketType = reader.ReadByte();
                PacketSize = reader.ReadUInt32();

                if (PacketSize > MAX_PACKET_SIZE)
                    throw new InvalidDataException("Size of packet more of:" + MAX_PACKET_SIZE);
            }
        }

        public byte[] ToBytes()
        {
            return (byte[])mBuffer.Clone();
        }

        public override string ToString()
        {
            return "Type: " + PacketType + " Size: " + PacketSize;
        }
    }

    public class Message 
    {
        public MessageHeader Header { get; protected set; }
        protected byte[] mBuffer;

        public Message(MessageHeader header, byte[] buffer)
        {
            Header = header;
            mBuffer = buffer;
        }

        public override string ToString()
        {
            return "MESSAGE: " + Header.ToString();
        }

        public byte[] ToBytes()
        {
            return (byte[])mBuffer.Clone();
        }
    }

    class MessageLogin : Message
    {
        public string Name {get; protected set;}
        public string Password { get; protected set; }

        public MessageLogin(MessageHeader header, byte[] buffer)
            :base(header, buffer)
        {
            using (MemoryStream stream = new MemoryStream(mBuffer))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                Name = reader.ReadString();
                Password = reader.ReadString();
            }
        }


        public override string ToString()
        {
            return base.ToString() + " LOGIN: " + Name + " PASSWORD: " + Password;
        }        
    }
}
