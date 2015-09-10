using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TheSilentNet.TLN
{
    public class AsynchIOServer
    {
        readonly TcpListener tcpListener = new TcpListener(new IPEndPoint (IPAddress.Any, 1998));
        
        void Listeners()
        {
            var cIpc = Database.Instance();
            Socket sock = tcpListener.AcceptSocket();
            if (sock.Connected) {
                Console.WriteLine ("SubNode {0} now connected to server.", sock.RemoteEndPoint);
                var networkStream = new NetworkStream (sock);
                var writer = new StreamWriter (networkStream);
                var reader = new StreamReader (networkStream);

                while (true)
                {
                    int request = Int32.Parse(reader.ReadLine());
                    Console.WriteLine("[SN]:" + request);
                    switch (request)
                    {
                        case 0:
                            // Register as a sub-node with the HLN
                            writer.WriteLine(1); // request valid, send data.
                            writer.Flush();
                            string subnode_type = reader.ReadLine();
                            CipNodeType nodetype = CipNodeType.AccessNode;
                            switch(subnode_type)
                            {
                                case "b":
                                    nodetype = CipNodeType.BottomNode;
                                    break;
                                default:
                                    nodetype = CipNodeType.AccessNode;
                                    break;
                            }
                            CipEntry subnode = new CipEntry(cIP.generatecIP(sock.RemoteEndPoint), nodetype);
                            cIpc.AddNode(subnode);
                            writer.WriteLine(1); // exchange over
                            writer.Flush();
                            break;
                        default:
                            writer.WriteLine(0); // 0 is an error code, telling the client that the request is invalid.
                            writer.Flush();
                            break;
                    }
                }
                reader.Close();
                networkStream.Close();
                writer.Close();
                

            }
            sock.Close();
            Console.WriteLine("Press any key to exit from server program");
            Console.ReadKey();
        }

        public void Start()
        {
            Database db = Database.Instance();
            IEnumerable<CipEntry> HLNs = db.GetTopLevelNodes(1024);
            tcpListener.Start();
            Console.WriteLine("[TLN] THIS IS A TOP LEVEL NODE");
            Console.WriteLine("[TLN] CURRENT HLNs IN cIPc:" + HLNs.Count());
            Console.WriteLine("[TLN] CONFIGURED TO ACCEPT 1024 SUB-NODES");
            int numberOfClientsYouNeedToConnect = 1024; 
            for (int i = 0; i < numberOfClientsYouNeedToConnect; i++)
            {
                Thread newThread = new Thread(new ThreadStart(Listeners));
                newThread.Start();
            }
        }
    }
}
