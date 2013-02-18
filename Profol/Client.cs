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

        public Queue<Message> messages = new Queue<Message>();

        public Client(TcpClient socket)
        {
            Socket = socket;
            StartRead();
        }

        public void StartRead()
        {
            Stream stream = Socket.GetStream();
            byte[] buffer = new byte[sizeof(byte) + sizeof(uint)];

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadHeaderCallback), buffer);
        }


        void ReadHeaderCallback(IAsyncResult result)
        {
            byte[] buffer = (byte[])result.AsyncState;
            Stream stream = Socket.GetStream();

            try
            {
                int bytesRead = stream.EndRead(result);
                if (bytesRead == buffer.Length)
                {
                    Message message = MessageFactory.CreateMessage(new MessageHeader(buffer));
                    Console.WriteLine(message);

                }
                else if(bytesRead < buffer.Length)
                {
                    stream.BeginRead(buffer, bytesRead, buffer.Length - bytesRead, new AsyncCallback(ReadHeaderCallback), buffer);
                }
                
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("PIZDEC:");
                Console.WriteLine(ex.Message);            	
            }
        }

        void ReadBodyCallback(IAsyncResult result)
        {
            /*Message message = (Message)result.AsyncState;
            Stream stream = Socket.GetStream();
            try
            {
                int bytesRead = stream.EndRead(result);

                if (bytesRead == message._buffer.Length)
                {
                    lock(messages)
                        messages.Enqueue(message);
                    Console.WriteLine("Packet was successfully received.");
                    Console.WriteLine(message);

                    StartRead();
                }
                else
                {
                    stream.BeginRead(message.buffer, bytesRead, message.buffer.Length - bytesRead, new AsyncCallback(ReadBodyCallback), message);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("PIZDEC:");
                Console.WriteLine(ex.Message);
            }*/
        }
    }
}
