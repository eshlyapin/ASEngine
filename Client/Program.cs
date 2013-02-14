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
            Client me = new Client("bot #" + rnd.Next(0,15), (float)rnd.NextDouble(), (float)rnd.NextDouble());

            Program prg = new Program();
            prg.Login(me, socket.GetStream());

            Console.WriteLine(me.ToString());

            while (true)
            {
                prg.WriteToStream(me, socket.GetStream());
                prg.ReadFromStream(socket.GetStream());

                Console.Clear();
                foreach (var client in _clientList)
                {
                    Console.WriteLine(client.ToString());
                }
            }            
        }

        public void Login(Client client, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(client.Name);
        }

        public void WriteToStream(Client client, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(client.x);
            writer.Write(client.y);
        }

        public void ReadFromStream(Stream stream)
        {
            Console.WriteLine("Waiting info about other users");
            BinaryReader reader = new BinaryReader(stream);
            string name = reader.ReadString();
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();

            if (isNewClient(name))
            {
                _clientList.Add(new Client(name, x, y));
            }
            else
            {
                var clients = from s in _clientList where (s.Name == name) select s;
                foreach (var client in clients)
                {
                    client.x = x;
                    client.y = y;
                }
            }
        }

        public bool isNewClient(string name)
        {
            foreach (var other in _clientList)
            {
                if (other.Name == name)
                    return false;
            }
            return true;
        }
    }
}
