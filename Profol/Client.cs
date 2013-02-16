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
    class Packet
    {
        byte[] buffer;
        public Packet()
        {
            buffer = new byte[1];

        }
    }

    class Client
    {
        public TcpClient Socket { get; protected set; }

        public Client(TcpClient socket)
        {
            Socket = socket;
            Socket.GetStream().BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), Socket.GetStream());
        }


        byte[] buffer = new byte[3];
        int offset = 0;

        void ReadCallback(IAsyncResult result)
        {
            Stream stream = (Stream)result.AsyncState;
            //using "using" will destroy the stream...
            //Dispose should be call only when conection was last.
            //using (Stream stream = (Stream)result.AsyncState)
            try
            {
                int bytesRead = stream.EndRead(result);
                offset += bytesRead;

                if (bytesRead > 0)
                {
                    if (offset >= buffer.Length)
                        offset = 0;
                    stream.BeginRead(buffer, offset, buffer.Length - offset, new AsyncCallback(ReadCallback), stream);
                }
                else
                {
                    //it is end of stream
                    stream.Dispose();
                    Socket = null;
                    Console.WriteLine("0 bytes read");
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("CONESCTION LOST");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
