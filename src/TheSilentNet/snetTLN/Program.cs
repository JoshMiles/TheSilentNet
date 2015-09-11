using System;
using System.Net;
using System.Reflection;
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
            Console.WriteLine ("TheSilentNet TopLevelNode Server v{0}", Assembly.GetEntryAssembly ().GetName ().Version);
            TopLevelNodeServer server = null;
            StatusAction ("TSN_LISTENER_CREATE", "Creating listener...", (()
                => server = new TopLevelNodeServer (IPAddress.Any, 1998)), ErrorTolerance.Fail);
            StatusAction ("TSN_LISTENER_START", "Starting listener...", server.Start, ErrorTolerance.Fail);
            Idle ();
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
                        Environment.Exit (1);
                        break;
                }
                return;
            }
            Console.WriteLine ("OK");
        }
    }
}
