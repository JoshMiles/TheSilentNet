using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace TheSilentNet
{
    public class cIP
    {
        string address = "";

        public static string generatecIP(System.Net.EndPoint RemoteEndPoint)
        {
            return RemoteEndPoint.ToString();
        }
    }
}
