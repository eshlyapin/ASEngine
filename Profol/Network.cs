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
        TcpListener mListener;

        List<Client> _clientList = new List<Client>();

        public Network(int port)
        {
            try
            {
                mListener = new TcpListener(IPAddress.Any, port);
                mListener.Start();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            mListener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallBack), null);
        }

        void AcceptCallBack(IAsyncResult result)
        {
            TcpClient socket = mListener.EndAcceptTcpClient(result);
            Client client = new Client(socket);
            lock(_clientList)
                _clientList.Add(client);
            Console.WriteLine("New connection");
            mListener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallBack), null);
        }

 
        public void Update()
        {

        }
    }
}
