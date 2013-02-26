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
        List<Client> mClientList = new List<Client>();

        public Network(int port)
        {
            try
            {
                mListener = new TcpListener(IPAddress.Any, port);
                mListener.Start();
                mListener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallBack), null);
            }
            catch(Exception ex)
            {
                throw ex;
            }            
        }

        void AcceptCallBack(IAsyncResult result)
        {
            TcpClient socket = mListener.EndAcceptTcpClient(result);
            Client client = new Client(socket);
            lock(mClientList)
                mClientList.Add(client);

            client.MessageReceived += ProccessLogin;

            Console.WriteLine("New connection");
            try
            {
                mListener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallBack), null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ProccessLogin(Client client, Message message)
        {
            if (message.Header.PacketType == 1)
                client.PushMessage(new LoginSuccessMessage());
        }

        public void Update()
        {
        }
    }
}
