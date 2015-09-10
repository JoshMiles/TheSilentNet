using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheSilentNet
{
	public class Client : Chainable<Client>
    {
        readonly Database db = Database.Instance ();
		readonly CipEntry self;

		public Client () : this (CipNodeType.AccessNode) { }

		public Client (CipNodeType nodeType) {
			self = new CipEntry ("", nodeType);
			checkFirstTime ();
		}

        /// <summary>
        /// A check to see if the client has an established cIP database, if no then a connection to a TLN is needed.
        /// </summary>
        void checkFirstTime () {
			var tlns = db.GetTopLevelNodes (1024);
            
			if (tlns.Any ()) {
                // There are no registered nodes in the cIPc
                // Therefore, we must assign the client to a TLN
            }
        }
    }
}
