using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Profol;

namespace nClient
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


            Client client = new Client(socket);


            while (true)
            {
                try
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.Read();
            }
        }


    }
}
