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
    class User
    {
        public int x, y;
        public string name;
        public override string ToString()
        {
            return name + " position{ " + x + ", " + y + " }";
        }
    }
    class Program
    {
        static List<User> users = new List<User>();

        static void Main(string[] args)
        {
            Random rnd = new Random();
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 1234);
            Stream stream = client.GetStream();

            string userName = "Bot #" + rnd.Next();

            StreamWriter wStream = new StreamWriter(stream);
            wStream.Write(userName.Length);
            wStream.Write(userName);

            int x = rnd.Next();
            int y = rnd.Next();


            while (true)
            {
                wStream.Write(x);
                wStream.Write(y);

                User nu = new User();
                byte[] buffer = new byte[8];
                stream.Read(buffer, 0, 8);
                nu.x = BitConverter.ToInt32(buffer, 0);
                nu.y = BitConverter.ToInt32(buffer, 3);

                stream.Read(buffer, 0, 4);
                int len = BitConverter.ToInt32(buffer,0);
                byte[] str = new byte[BitConverter.ToInt32(buffer, 0)];
                stream.Read(str, 0, len);
                nu.name = BitConverter.ToString(str, 0);

                bool needToAdd = true;
                Console.Clear();
                for (int i = 0; i < users.Count; ++i)
                {
                    if (nu.name == users[i].name)
                        needToAdd = false;
                    Console.WriteLine(users[i].ToString());
                }
                if (needToAdd)
                    users.Add(nu);
            }            
        }
    }
}
