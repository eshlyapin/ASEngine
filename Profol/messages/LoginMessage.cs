using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Profol
{
    public class LoginMessage : Message
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public LoginMessage(MessageHeader header, byte[] buffer)
        {
            Header = header;
            Parse(buffer);
        }

        public LoginMessage(string username, string password)
        {
            Username = username;
            Password = password;
            //it doesn't look fine
            //have to refactor this.
            Header = new MessageHeader(1, (uint)(Username.Length + Password.Length + 2));
        }


        public void Parse(byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                Username = reader.ReadString();
                Password = reader.ReadString();
            }
        }

        public override byte[] ToBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Header.ToBytes());
                writer.Write(Username);
                writer.Write(Password);
                return stream.ToArray();
            }
        }

        public override string ToString()
        {
            return "Message: " + Username + " : " + Password;
        }
    }
}
