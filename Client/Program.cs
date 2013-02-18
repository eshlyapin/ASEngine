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
        static void ExecFlood(Stream stream)
        {
            Random rnd = new Random();
            byte[] buffer = new byte[rnd.Next(1,4)];
            rnd.NextBytes(buffer);
            try
            {
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            foreach (var b in buffer)
                Console.Write(b + " ");
            Console.WriteLine();
        }

        static void ExecLogin(Stream stream, string username, string password)
        {
            try
            {
                byte[] buffer = new byte[sizeof(byte) + sizeof(uint) + username.Length + password.Length + 2];
                MemoryStream memStream = new MemoryStream(buffer);
                BinaryWriter writer = new BinaryWriter(memStream);
                writer.Write((byte)1);
                writer.Write((uint)buffer.Length - 5);
                writer.Write(username);
                writer.Write(password);
                stream.Write(buffer, 0, buffer.Length);

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
            
            Console.WriteLine("Enter host ip:");
            string ip = Console.ReadLine();
            if (ip == "")
                ip = "127.0.0.1";

            try
            {
                socket.Connect(ip, 30000);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
                return;
            }

            Random rnd = new Random(DateTime.Now.Second);
            
            string prevCommand = "";
            Console.WriteLine("Available commands:");
            Console.WriteLine("login-fake");
            Console.WriteLine("login-user");
            Console.WriteLine("flood");
            Console.WriteLine("reconnect");
            Console.WriteLine("repeat");
            Console.WriteLine();
            while (true)
            {
                Console.Write(">");
                string command = Console.ReadLine();
                if (command == "repeat")
                {
                    Console.Write("count: ");
                    string sCount = Console.ReadLine();
                    int count = Convert.ToInt32(sCount);
                    for (int i = 0; i < count; ++i)
                        Exec(socket.GetStream(), prevCommand);
                }
                else if (command == "reconnect")
                {
                    socket.Close();
                    socket = new TcpClient();
                    try
                    {
                        socket.Connect(ip, 30000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Exec(socket.GetStream(), command);
                }
                prevCommand = command;
            }  
        }

        static void Exec(Stream stream, string command)
        {
            if (command == "flood")
                ExecFlood(stream);
            if (command == "login-fake")
                ExecLogin(stream, "ololoev", "123456");
            if (command == "login-user")
            {
                Console.Write("User: ");
                string name = Console.ReadLine();
                Console.Write("Password: ");
                string pass = Console.ReadLine();
                ExecLogin(stream, name, pass);
            }
        }
    }
}
