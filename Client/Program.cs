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
    class Client
    {
        public string Name;
        public float x = 0;
        public float y = 0;

        public Client(string name, float x, float y)
        {
            Name = name;
            this.x = x;
            this.y = y;
        }
        public override string ToString()
        {
            return Name + " position{ " + x + ", " + y + " }";
        }

        public static bool operator ==(Client left, Client right)
        {
            return left.Name == right.Name;
        }

        public static bool operator !=(Client left, Client right)
        {
            return !(left == right);
        }
    }

    class Program
    {
        static List<Client> _clientList = new List<Client>();

        static void Main(string[] args)
        {
            TcpClient socket = new TcpClient();
            try
            {
                socket.Connect("127.0.0.1", 1234);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Random rnd = new Random(DateTime.Now.Second);

            int i = 0;
            while (true)
            {
                i++;
                byte[] buffer = new byte[9];
                rnd.NextBytes(buffer);
                socket.GetStream().Write(buffer, 0, buffer.Length);


                if (Console.KeyAvailable == true)
                {
                    Console.WriteLine("Sended bytes: " + i * 9);

                    foreach (var a in buffer)
                        Console.Write(a + " ");

                    Console.Read();
                    socket.Close();
                }
                //Console.Read();//Thread.Sleep(500);
            }  
        }
    }
}
