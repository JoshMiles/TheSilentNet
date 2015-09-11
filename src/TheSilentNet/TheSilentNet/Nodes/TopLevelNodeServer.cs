using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TheSilentNet {
    public class TopLevelNodeServer {

        const int MAX_CONNECTIONS = 1024;
        const int MAX_CONCURRENT_CONNECTIONS = 4;

        readonly TcpListener listener;
        readonly NodeInformation info;
        readonly CipEntry self;

        volatile int total_connections;
        volatile int connections_kept;

        public delegate void NodeStateChangedEventArgs (CipEntry node);
        public event NodeStateChangedEventArgs NodeConnected;
        public event NodeStateChangedEventArgs NodeDisconnected;

        public delegate void UpdateTriggeredEventArgs (NodeInformation info);
        public event UpdateTriggeredEventArgs UpdateTriggered;

        CancellationTokenSource tksource;

        public TopLevelNodeServer (IPAddress ip, int port) {
            listener = new TcpListener (ip, port);
            tksource = new CancellationTokenSource ();
            self = CipEntry.GenerateFor (new IPEndPoint (ip, port), CipNodeType.TopLevelNode);
            info = new NodeInformation (self, MAX_CONNECTIONS, MAX_CONCURRENT_CONNECTIONS);
            NodeConnected += TopLevelNodeServer_NodeConnected;
            NodeDisconnected += TopLevelNodeServer_NodeDisconnected;
            UpdateTriggered += delegate { };
        }

        public TopLevelNodeServer (string ip, int port)
            : this (IPAddress.Parse (ip), port) { }

        public NodeInformation QueryInformation () => info;

        public void Start () {
            listener.Start ();
            for (int i = 0; i < MAX_CONCURRENT_CONNECTIONS; i++)
                Task.Factory.StartNew (Listen, tksource.Token);
            Task.Factory.StartNew (Update);
        }

        public void Stop () {
            listener.Stop ();
            tksource.Cancel ();
        }

        void Update () {
            while (true) {
                info.Update (total_connections, connections_kept);
                UpdateTriggered (info);
                Thread.Sleep (100);
            }
        }

        // TODO: Properly implement this
        void TopLevelNodeServer_NodeConnected (CipEntry node) {
            ++connections_kept;
            ++total_connections;
        }

        // TODO: Properly implement this
        void TopLevelNodeServer_NodeDisconnected (CipEntry node)
            => --connections_kept;

        void Listen () {
            while (true) {

                // Return if cancellation was requested.
                if (tksource.IsCancellationRequested)
                    return;

                var sock = listener.AcceptTcpClient ();

                var node = CipEntry.GenerateFor ((IPEndPoint)sock.Client.RemoteEndPoint);
                NodeConnected (node);

                using (var reader = new StreamReader (sock.GetStream ()))
                using (var writer = new StreamWriter (sock.GetStream ())) {
                    // Do something like a handshake here
                    // to determine the type of the connecting node
                }

                sock.Close ();
                sock.Client.Close ();
                NodeDisconnected (node);
            }
        }
    }
}
