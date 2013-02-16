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
    class Network
    {
        TcpListener _listener;

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
            _listener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallBack), null);
        }

        void AcceptCallBack(IAsyncResult result)
        {
            TcpClient socket = _listener.EndAcceptTcpClient(result);
            Client client = new Client(socket);
            lock(_clientList)
                _clientList.Add(client);
            _listener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallBack), null);
        }

 
        public void Update()
        {

        }
    }
}
