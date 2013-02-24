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
    public class Client
    {
        Queue<Message> readQueue = new Queue<Message>();
        Queue<Message> writeQueue = new Queue<Message>();

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

        public void PushMessage(Message item)
        {
            writeQueue.Enqueue(item);
            WriteNewMessage();
        }

        public Message PullMessage()
        {
            return readQueue.Dequeue();
        }

        protected void ReadNewMessage()
        {
            Stream stream = Stream.Null;
            try
            {
                stream = mTcpSocket.GetStream();
                ClientState state = new ClientState();
                state.buffer = new byte[MessageHeader.HEADER_SIZE];
                stream.BeginRead(state.buffer, state.offset, state.buffer.Length, ReadCallback, state);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                mTcpSocket.Close();
            }
        }

        protected void WriteNewMessage()
        {
            try
            {
                if (writeQueue.Count > 0)
                {
                    Stream stream = mTcpSocket.GetStream();
                    Message message = writeQueue.Dequeue();

                    using(MemoryStream ms = new MemoryStream())
                    using(BinaryWriter br = new BinaryWriter(ms))
                    {
                        br.Write(message.Header.ToBytes());
                        br.Write(message.ToBytes());

                        byte[] buffer = ms.ToArray();

                        stream.BeginWrite(buffer, 0, buffer.Length, WriteCallback, buffer);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void WriteCallback(IAsyncResult result)
        {
            Stream stream = mTcpSocket.GetStream();
            byte[] buffer = (byte[])result.AsyncState;

            try
            {
                stream.EndWrite(result);
                WriteNewMessage();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        protected void ReadCallback(IAsyncResult result)
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
                        int bytesToRead = state.buffer.Length - state.offset;
                        stream.BeginRead(state.buffer, state.offset, bytesToRead, ReadCallback, state);
                    }
                    else
                    {
                        if (state.header == null)
                        {
                            state.header = new MessageHeader(state.buffer);
                            state.buffer = new byte[state.header.PacketSize];
                            state.offset = 0;
                            stream.BeginRead(state.buffer, state.offset, state.buffer.Length, ReadCallback, state);
                        }
                        else
                        {
                            Message message = MessageFactory.CreateMessage(state.header, state.buffer);
                            lock (readQueue)
                                readQueue.Enqueue(message);
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
                mTcpSocket.Close();
            }
        }
    }
}
