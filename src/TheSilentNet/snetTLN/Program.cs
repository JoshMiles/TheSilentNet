using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using TheSilentNet;

namespace snetTLN
{
    class Program
    {
        [Flags]
        enum ErrorTolerance {
            Notify  = 1 << 1,
            Ignore  = 1 << 2,
            Fail    = 1 << 3,
        }

        static void Main (string[] args) {
            Console.WriteLine ("TheSilentNet Server");
            TopLevelNodeServer server = null;
            StatusAction ("TSN_LISTENER_CREATE", "Creating listener...", (()
                => server = new TopLevelNodeServer (IPAddress.Any, 1998)), ErrorTolerance.Fail);
            StatusAction ("TSN_LISTENER_SUBSCRIBE", "Subscribing to listener events...", (()
                => server.UpdateTriggered += UpdateGraph), ErrorTolerance.Notify);
            StatusAction ("TSN_LISTENER_START", "Starting listener...", server.Start, ErrorTolerance.Fail);
            Idle ();
        }

        static void UpdateGraph (NodeInformation info) {
            var buffer = new StringBuilder ();
            buffer.Append ("TheSilentNet Server\n\n");
            buffer.AppendLine (WriteCat ("Property") + WriteCat ("Primary") + WriteCat ("Secondary"));
            buffer.AppendLine (string.Empty.PadLeft (20 * 3, '='));
            buffer.AppendLine (WriteCat ("Uptime") + WriteCat ("{0:hh':'mm':'ss}", info.Uptime));
            buffer.AppendLine (WriteCat ("Node") + WriteCat (info.CipId) + WriteCat (info.CipTypeString));
            buffer.AppendLine (WriteCat ("Connections") + WriteCat ("{0} / {1}", info.CurrentlyHeldClients, info.TotalAllowedClients) + WriteCat ("{0} concurrent", info.MaxConcurrentClients));
            Console.Clear ();
            Console.Write (buffer);
        }

        static string WriteCat (string format, params object[] args) {
            var msg = string.Format (format, args).PadRight (20, ' ');
            return string.Format ("{0}", msg);
        }

        static void Idle () {
            Thread.Sleep (-1);
        }

        static void StatusAction (string task, string msg, Action act, ErrorTolerance tolerance = ErrorTolerance.Notify) {
            Console.Write (msg);
            try {
                act ();
            } catch (Exception e) {
                Console.WriteLine ("FAIL");
                switch (tolerance) {
                    case ErrorTolerance.Notify:
                        Console.WriteLine ("[INFO] Task '{0}' failed. Reason:\n\t{1}", task, e.Message);
                        break;
                    case ErrorTolerance.Fail:
                        Console.WriteLine ("[SEVERE] '{0}' failed. Reason:\n\t{1}", task, e.Message);
                        throw e;
                }
                return;
            }
            Console.WriteLine ("OK");
        }
    }
}
