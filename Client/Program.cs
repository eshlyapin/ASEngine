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
        static void SendMessage(Stream stream, string type)
        {
            Random rnd = new Random();
            Thread.Sleep(50);
            byte[] mbuf;

            if (type == "m1")
            {
                mbuf = new byte[6];
                rnd.NextBytes(mbuf);
                mbuf[0] = 1;
            }
            else if (type == "m2")
            {
                mbuf = new byte[11];
                rnd.NextBytes(mbuf);
                mbuf[0] = 2;
            }
            else if (type == "m3")
            {
                mbuf = new byte[16];
                rnd.NextBytes(mbuf);
                mbuf[0] = 3;
            }
            else if (type == "login")
            {
                string login = "eshlyapin";
                string pswd = "ololo";

                MemoryStream ms = new MemoryStream();
                BinaryWriter bs = new BinaryWriter(ms);
                bs.Write((byte)1);
                bs.Write(login.Length + pswd.Length + 2);
                bs.Write(login);
                bs.Write(pswd);
                mbuf = ms.ToArray();
            }
            else
            {
                mbuf = new byte[rnd.Next(1, 20)];
                rnd.NextBytes(mbuf);
                mbuf[0] = (byte)rnd.Next(1, 7);
            }
            try
            {
                stream.Write(mbuf, 0, mbuf.Length);
                Console.Write("TYPE:" + mbuf[0]);
                Console.Write(" DATA: ");
                for (int i = 1; i < mbuf.Length; ++i)
                {
                    Console.Write(mbuf[i] + " ");
                }
                Console.WriteLine();
                
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("send fail");
                Console.WriteLine(ex.Message);
            }
        }

        static void Main(string[] args)
        {
            TcpClient socket = new TcpClient();
            try
            {
                Console.WriteLine("Enter host ip:");
                string ip = Console.ReadLine();
                socket.Connect(ip, 30000);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
                return;
            }

            Random rnd = new Random(DateTime.Now.Second);

            Console.WriteLine("Client application for testing");
            Console.WriteLine("send command description:\n\t");
            Console.WriteLine("send <message type> [ -r <repeat> ]");
            Console.WriteLine("message types: m1, m2, m3");
            Console.WriteLine("if message type is illegal, random data will be sent");
            while (true)
            {
                string line = Console.ReadLine();
                string[] command = line.Split(' ');
                if (command.Length > 1)
                {
                    if (command[0] == "send")
                    {
                        SendMessage(socket.GetStream(), command[1]);
                        if (command.Length > 3)
                        {
                            if (command[2] == "-r")
                            {
                                try
                                {
                                    int count = Convert.ToInt32(command[3]);
                                    for (int i = 0; i < count - 1; ++i)
                                    {
                                        SendMessage(socket.GetStream(), command[1]);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    Console.WriteLine("invalid format");
                                }
                            }
                        }
                    }
                }
            }  
        }
    }
}
