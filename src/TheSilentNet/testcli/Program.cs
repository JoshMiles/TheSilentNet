using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TheSilentNet;

namespace testcli
{
	class MainClass {
		public static void Main (string[] args) {

			// Get instance
			var db = Database.Instance;

			// Wipe db
			db.Recreatedb ();

			// Wrap test into transaction for efficiency
			db.WrapTransaction (() => {
				
				// Create 1024 top level nodes
				for (var i = 0; i < 1024; i++)
					db.AddNode (new CipEntry (Guid.NewGuid ().ToString (), CipNodeType.TopLevelNode));
				
				// Create 1024 a nodes
				for (var i = 0; i < 1024; i++)
					db.AddNode (new CipEntry (Guid.NewGuid ().ToString (), CipNodeType.AccessNode));

                // Create 1024 b nodes
                for (var i = 0; i < 1024; i++)
                    db.AddNode (new CipEntry (Guid.NewGuid ().ToString (), CipNodeType.BottomNode));
            });

			Console.WriteLine ("===\nDone.\n===");

			// Get all results
			db.WrapFast (() => {
				var query = Query.GrabNew ().Select ("*").From ("cips");
				var nodes = new List<CipEntry> ();
				using (var reader = db.ExecReader (query)) {
					if (reader.HasRows)
						while (reader.Read ())
							nodes.Add (new CipEntry (reader.GetString (0), reader.GetByte (1)));
				}
				foreach (var cip in nodes)
					Console.WriteLine (cip);
			});
		}
	}
}
