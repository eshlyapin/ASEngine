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
    class Program
    {

        static void Main(string[] args)
        {
            Network network = new Network(1234);
            while (true)
            {
                network.Update();
            }

        }
    }
}
