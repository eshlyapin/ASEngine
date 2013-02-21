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
        Queue<Message> messages = new Queue<Message>();
        TcpClient mTcpSocket;

        class ClientState
        {
            public byte[] buffer = null;
            public MessageHeader header = null;
            public Message message = null;
            public int offset = 0;
        }

        public Client(TcpClient socket)
        {
            mTcpSocket = socket;
            ReadNewMessage();
        }

        void ReadNewMessage()
        {
            Stream stream = Stream.Null;
            try
            {
                stream = mTcpSocket.GetStream();
                ClientState state = new ClientState();
                state.buffer = new byte[MessageHeader.HeaderSize];
                stream.BeginRead(state.buffer, state.offset, state.buffer.Length, ReadCallback, state);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                stream.Close();
                mTcpSocket.Close();
            }
        }

        void ReadCallback(IAsyncResult result)
        {
            Stream stream = mTcpSocket.GetStream();
            ClientState state = (ClientState)result.AsyncState;

            try
            {
                int bytesRead = stream.EndRead(result);
                if (bytesRead == 0)
                {
                    Console.WriteLine("End of stream");
                    mTcpSocket.Close();
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
                            lock(messages)
                                messages.Enqueue(message);
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
                stream.Close();
                mTcpSocket.Close();
            }
        }
    }
}
