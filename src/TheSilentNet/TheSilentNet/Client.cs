using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheSilentNet
{
    public class Client
    {
        Database db = Database.Instance();
        char NODE_TYPE = 'a'; // set to 'a' by default

        public Client(char NODE_TYPE)
        {
            // Client Settings
            this.NODE_TYPE = NODE_TYPE;

            checkFirstTime(); // see summary of function
        }
        /// <summary>
        /// A check to see if the client has an established cIP database, if no then a connection to a TLN is needed.
        /// </summary>
        void checkFirstTime()
        {
            IEnumerable<CipEntry> NODES = db.GetNodes(1024, true);
            
            if(NODES.Count<CipEntry>() == 0)
            {
                // There are no registered nodes in the cIPc
                // Therefore, we must assign the client to a TLN
                
            }
        }
    }
}
