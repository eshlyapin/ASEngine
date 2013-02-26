using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Profol
{
    public class MessageHeader
    {
        public uint PacketSize {get;set;}
        public byte PacketType {get;set;}
        public const uint HEADER_SIZE = 5;

        public MessageHeader()
            :this(0,0)
        {
        }

        public MessageHeader(byte type, uint size)
        {
            const uint maxPacketSize = 4096;

            if(size > maxPacketSize)
                throw new ArgumentOutOfRangeException("invalid size of packet");

            PacketType = type;
            PacketSize = size;
        }

        public MessageHeader(byte[] buffer)
        {
            Parse(buffer);
        }

        public void Parse(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                PacketType = reader.ReadByte();
                PacketSize = reader.ReadUInt32();
            }
        }

        public byte[] ToBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(PacketType);
                writer.Write(PacketSize);
                return stream.ToArray();
            }
        }
    }
}
