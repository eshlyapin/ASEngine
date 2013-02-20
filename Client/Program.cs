using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClient socket = new TcpClient();

            Console.WriteLine("Enter host ip:");
            string ip = Console.ReadLine();
            if (ip == "")
                ip = "127.0.0.1";

            try
            {
                socket.Connect(ip, 30000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
                return;
            }

            Random rnd = new Random(DateTime.Now.Second);


            while (true)
            {
                try
                {
                    //byte[] buffer = new byte[50];
                    //rnd.NextBytes(buffer);
                    byte[] buffer = LoginMessage();
                    socket.GetStream().Write(buffer, 0, buffer.Length);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.Read();
            }
        }

        static byte[] LoginMessage()
        {
            string name = "ololo";
            string password = "pswd";
            int size = name.Length + password.Length + 2;
            
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write((byte)1);
            writer.Write(size);
            writer.Write(name);
            writer.Write(password);

            return stream.ToArray();
        }
    }
}
