using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSilentNet;
using System.Net;
using System.Net.Sockets;

namespace snetTLN
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up HLN");
            TheSilentNet.TLN.AsynchIOServer server = new TheSilentNet.TLN.AsynchIOServer();
            server.Start();
        }
    }
}
