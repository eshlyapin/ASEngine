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
        Queue<Message> _messages = new Queue<Message>();
        TcpClient _tcpSocket;



        class ClientState
        {
            public byte[] buffer = null;
            public MessageHeader header = null;
            public Message message = null;
            public int offset = 0;
        }

        public Client(TcpClient socket)
        {
            _tcpSocket = socket;
            ReadNewMessage();
        }

        void ReadNewMessage()
        {
            Stream stream = _tcpSocket.GetStream();
            ClientState state = new ClientState();
            state.buffer = new byte[MessageHeader.HeaderSize];
            stream.BeginRead(state.buffer, state.offset, state.buffer.Length, ReadCallback, state);
        }


        void ReadCallback(IAsyncResult result)
        {
            Stream stream = _tcpSocket.GetStream();
            ClientState state = (ClientState)result.AsyncState;

            try
            {
                int bytesRead = stream.EndRead(result);
                if (bytesRead == 0)
                {
                    Console.WriteLine("End of stream");
                    _tcpSocket.Close();
                    return;
                }
                else
                {
                    state.offset += bytesRead;
                    if (state.offset < state.buffer.Length)
                    {
                        stream.BeginRead(state.buffer, state.offset, bytesRead, ReadCallback, state);
                    }
                    else
                    {
                        if (state.header == null)
                        {
                            state.header = new MessageHeader(state.buffer);
                            state.buffer = new byte[state.header.PacketSize];
                            state.offset = 0;
                            stream.BeginRead(state.buffer, state.offset, state.buffer.Length - state.offset, ReadCallback, state);
                        }
                        else
                        {
                            Message message = MessageFactory.CreateMessage(state.header, state.buffer);
                            lock(_messages)
                                _messages.Enqueue(message);
                            Console.WriteLine("Message Recieved");
                            Console.WriteLine(message);
                            ReadNewMessage();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAIL!");
                Console.WriteLine(ex.Message);
                _tcpSocket.Close();
            }
        }
    }
}
