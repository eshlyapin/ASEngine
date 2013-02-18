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
    delegate void MessageHandler(byte[] buffer, object state);
    class AsyncReader
    {
        public TcpClient _tcpSocet;

        class State
        {
            public byte[] buffer;
            public MessageHandler callback;
            public object state;

            public State(byte[] buffer, MessageHandler callback, object state)
            {
                this.buffer = buffer;
                this.callback = callback;
                this.state = state;
            }
        }

        public AsyncReader(TcpClient socket)
        {
            _tcpSocet = socket;
        }

        public void BeginRead(byte[] buffer, MessageHandler callback, object state)
        {
            Stream stream = _tcpSocet.GetStream();
            IAsyncResult res = stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), new State(buffer,callback, state));
            
        }

        void ReadCallback(IAsyncResult result)
        {
            State state = (State)result.AsyncState;
            Stream stream = _tcpSocet.GetStream();

            if (!result.AsyncWaitHandle.WaitOne(3000))
                Console.WriteLine("Jigurda...");

            try
            {
                int bytesRead = stream.EndRead(result);
                if (bytesRead == 0)
                {
                    Console.WriteLine("end of stream");
                    _tcpSocet.Close();
                }
                else
                {
                    if (bytesRead == state.buffer.Length)
                        state.callback.Invoke(state.buffer, state.state);
                    else
                    {
                        stream.BeginRead(state.buffer, bytesRead, state.buffer.Length - bytesRead, ReadCallback, state);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
