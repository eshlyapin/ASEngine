using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Profol
{
    class UserData
    {
        public float x, y;
    }

    class User
    {
        public UserData Data { get; set; }
        public bool Loggined = false;
        public string Name { get; set; }
    }

    class Program
    {
        TcpListener listener;
        List<TcpClient> clientList = new List<TcpClient>();
        Dictionary<TcpClient, User> users = new Dictionary<TcpClient, User>();

        public Program()
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8089);
            try
            {
                listener.Start(10);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Thread acceptThread = new Thread(
                () =>
                {
                    TcpClient client = listener.AcceptTcpClient();
                    lock (clientList)
                    {
                        clientList.Add(client);
                    }
                });
            acceptThread.Start();
        }

        bool Update()
        {
            if (clientList.Count > 0)
            {
                foreach (var client in clientList)
                {
                    User user = users[client];
                    if (!user.Loggined)
                    {
                        try
                        {
                            ProcessLogin(user, client.GetStream());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        user.Data = ParseData(client.GetStream());
                        foreach (var otherUser in users)
                        {
                            var otherPlayer = otherUser.Value;
                            if(otherPlayer != user)
                                SendUsersData(otherPlayer, client.GetStream());
                        }
                    }
                }
            }

            return true;
        }

        void SendUsersData(User user, Stream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(user.Data.x);
            writer.Write(user.Data.y);
            writer.Write(user.Name.Length);
            writer.Write(user.Name);
        }

        UserData ParseData(Stream stream)
        {
            UserData data = new UserData();

            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, 8);

            data.x = BitConverter.ToInt32(buffer, 0);
            data.y = BitConverter.ToInt32(buffer, 3);

            return data;
        }

        void ProcessLogin(User user, Stream stream)
        {
            int len = 0;
            byte[] lenbuf = new byte[4];
            stream.Read(lenbuf,0,4);
            len = BitConverter.ToInt32(lenbuf, 0);
            if (len > -1)
            {
                byte[] buffer = new byte[len];
                int count = stream.Read(buffer, 0, len);
                if (count != len)
                    throw new Exception("Data wasn't recived blyad'!");

                user.Name = BitConverter.ToString(buffer);
                user.Loggined = true;
            }
        }


        static void Main(string[] args)
        {
            while((new Program()).Update());
        }
    }
}
