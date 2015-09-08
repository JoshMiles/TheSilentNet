using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using TheSilentNet;

namespace TheSilentNet.TLN
{
    public class AsynchIOServer
    {
        TcpListener tcpListener = new TcpListener(1998);
        
        void Listeners()
        {
            Database cIpc = Database.Instance();
            Socket socketForClient = tcpListener.AcceptSocket();
            if (socketForClient.Connected)
            {
                Console.WriteLine("Sub-node:" + socketForClient.RemoteEndPoint + " now connected to server.");
                NetworkStream networkStream = new NetworkStream(socketForClient);
                System.IO.StreamWriter streamWriter =
                new System.IO.StreamWriter(networkStream);
                System.IO.StreamReader streamReader =
                new System.IO.StreamReader(networkStream);

                while (true)
                {
                    int request = Int32.Parse(streamReader.ReadLine());
                    Console.WriteLine("[SN]:" + request);
                    switch (request)
                    {
                        case 0:
                            // Register as a sub-node with the HLN
                            streamWriter.WriteLine(1); // request valid, send data.
                            streamWriter.Flush();
                            string subnode_type = streamReader.ReadLine();
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
                            CipEntry subnode = new CipEntry(cIP.generatecIP(socketForClient.RemoteEndPoint), nodetype);
                            cIpc.AddNode(subnode);
                            streamWriter.WriteLine(1); // exchange over
                            streamWriter.Flush();
                            break;
                        default:
                            streamWriter.WriteLine(0); // 0 is an error code, telling the client that the request is invalid.
                            streamWriter.Flush();
                            break;
                    }
                }
                streamReader.Close();
                networkStream.Close();
                streamWriter.Close();
                

            }
            socketForClient.Close();
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
