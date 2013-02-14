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
    class Client
    {
        public TcpClient Socket { get; protected set; }

        public bool isLogged = false;
        public string Name;
        public float x = 0;
        public float y = 0;

        public Client(TcpClient socket)
        {
            Socket = socket;
        }

        public override string ToString()
        {
            return "Client: " + Name + "{ " + x + ", " + y + " }";
        }
    }

    class Network
    {
        TcpListener _listener;
        bool _isRunned = false;

        List<Client> _clientList = new List<Client>();

        public Network(int port)
        {
            try
            {
                _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                _listener.Start();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            _isRunned = true;
            Thread acceptThread = new Thread(AcceptMethod);
            acceptThread.Start();
        }

        void AcceptMethod()
        {
            while (_isRunned)
            {
                TcpClient socket = _listener.AcceptTcpClient();
                lock (_clientList)
                {
                    _clientList.Add(new Client(socket));
                }
            }
        }

        public void WriteToStream(Client client, Stream stream)
        {
            if (client.isLogged)
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(client.Name);
                writer.Write(client.x);
                writer.Write(client.y);
            }
        }

        public void ReadFromStream(Client client, Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            if (!client.isLogged)
            {
                client.Name = reader.ReadString();
                client.isLogged = true;
            }
            else
            {
                client.x = reader.ReadSingle();
                client.y = reader.ReadSingle();
            }
        }
        public void Update()
        {
            //Console.Clear();
            lock (_clientList)
            {
                foreach (var client in _clientList)
                {
                    Console.WriteLine("Waiting for new data!");
                    ReadFromStream(client, client.Socket.GetStream());

                    var otherClients = from oc in _clientList where client != oc select oc;
                    foreach (var other in otherClients)
                    {
                        WriteToStream(client, other.Socket.GetStream());
                    }
                    Console.WriteLine(client.ToString());
                }
            }
        }
    }
}
