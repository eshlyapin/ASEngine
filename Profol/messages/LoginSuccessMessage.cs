using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Profol
{
    class LoginSuccessMessage : Message
    {
        public LoginSuccessMessage()
        {
            string okay = "Welcome";
            Header = new MessageHeader(2, (uint)okay.Length + 1);
        }

        public LoginSuccessMessage(MessageHeader header, byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                Console.WriteLine("Created Success message: " + reader.ReadString());
            }
        }

        public override byte[] ToBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Header.ToBytes());
                writer.Write("Welcome");
                return stream.ToArray();
            }
        }

        public override string ToString()
        {
            return "OK";
        }
    }
}
