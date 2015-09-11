using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheSilentNet {
    public class NodeInformation {

        readonly CipEntry entry;
        public string CipId { get { return entry.Value; } }
        public string CipTypeString { get { return entry.Type.ToString (); } }

        TimeSpan uptime;
        public TimeSpan Uptime { get { return uptime; } }

        public int TotalAcceptedClients { get; private set; }
        public int CurrentlyHeldClients { get; private set; }
        public int TotalAllowedClients { get; private set; }
        public int MaxConcurrentClients { get; private set; }

        DateTime now;

        public NodeInformation (CipEntry cip, int total_allowed_clients, int max_concurrent_clients) {
            entry = cip;
            uptime = new TimeSpan (0, 0, 0, 0, 0);
            now = DateTime.Now;
            TotalAllowedClients = total_allowed_clients;
            MaxConcurrentClients = max_concurrent_clients;
        }

        public void Update (int total_clients, int currently_held_clients) {
            TotalAcceptedClients += total_clients;
            CurrentlyHeldClients = currently_held_clients;
            var new_now = DateTime.Now;
            var diff = new_now.Subtract (now);
            now = new_now;
            uptime = uptime.Add (diff);
        }
    }
}
