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
        public Queue<Message> messages = new Queue<Message>();

        AsyncReader reader;

        public Client(TcpClient socket)
        {
            reader = new AsyncReader(socket);
            ReadNewPacket();
        }

        void ReadNewPacket()
        {
            byte[] headerBuffer = new byte[MessageHeader.HeaderSize];
            reader.BeginRead(headerBuffer, new MessageHandler(OnHeaderRead), null);
        }


        void OnHeaderRead(byte[] buffer, object state)
        {
            try
            {
                MessageHeader header = new MessageHeader((byte[])buffer);
                byte[] bodyBuffer = new byte[header.PacketSize];
                reader.BeginRead(bodyBuffer, OnBodyRead, header);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client should be disconected: " + ex.Message);
                reader._tcpSocet.Close();
            }
        }

        void OnBodyRead(byte[] buffer, object state)
        {
            try
            {
                Message message = MessageFactory.CreateMessage((MessageHeader)state, buffer);
                messages.Enqueue(message);
                Console.WriteLine("Message Recieved:");
                Console.WriteLine(message);
                ReadNewPacket();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client should be disconected: " + ex.Message);
                reader._tcpSocet.Close();
            }
        }
    }
}
