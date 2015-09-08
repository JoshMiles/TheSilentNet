using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.Linq;

namespace TheSilentNet
{
	/// <summary>
	/// Database connector.
	/// </summary>
	public class Database : Singleton<Database>, IDisposable
	{
		/// <summary>
		/// The SQLite data source.
		/// </summary>
		const string DATA_SOURCE = "cipc.db";

		/// <summary>
		/// The initial table layout.
		/// </summary>
		const string TABLE_LAYOUT =
			
			// Enable foreign key support.
			"PRAGMA foreign_keys = ON;" +
			// Create the table 'cips' only if it doesn't yet exist.
			"CREATE TABLE IF NOT EXISTS cips (" +
			// The cIP must not be null and is unique.
			// The query must fail on collision.
			"cip VARCHAR(512) NOT NULL UNIQUE ON CONFLICT FAIL," +
			// The TLN blob indicates whether a node is a TLN.
			"tln BLOB(1) DEFAULT 0" +
			");" +
			// Create the table 'domains' only if it doesn't yet exist.
			"CREATE TABLE IF NOT EXISTS domains (" +
			// This column is a reference to the column of same name
			// in the 'cips' table (defined above).
			"cip VARCHAR(512) NOT NULL UNIQUE ON CONFLICT FAIL," +
			// The name of the domain must not be null and is unique.
			// The query must fail on collision.
			"name VARCHAR(64) NOT NULL UNIQUE ON CONFLICT FAIL," +
			// The port of the host defaults to 80.
			"port INT DEFAULT 80," +
			// Make the 'cip' field reference the 'cip' field
			// in the 'cips' table (defined above).
			"FOREIGN KEY(cip) REFERENCES cips(cip)" +
			");";

		#region Premade queries

		/// <summary>
		/// Selects the cIPs of all TLNs from the 'cips' table.
		/// </summary>
		readonly string QUERY_SELECT_TLN = Instance ().Select ("cip").From ("cips").Where ("tln").Eq (1);

		/// <summary>
		/// Selects the cIPs of all nodes (excluding TLNs) from the 'cips' table.
		/// </summary>
		readonly string QUERY_SELECT_KNOWN_SN = Instance ().Select ("cip").From ("cips").Where ("tln").Eq (0);

		/// <summary>
		/// Inserts the cIP of a TLN into the 'cips' table.
		/// </summary>
		readonly string QUERY_INSERT_TLN = Instance ().Into ("cips").Insert ("cip", "tln").Values ("@cip", 1);

		/// <summary>
		/// Inserts the cIP of a non-TLN into the 'cips' table.
		/// </summary>
		readonly string QUERY_INSERT_SN = Instance ().Into ("cips").Insert ("cip").Values ("@cip");

		#endregion

		/// <summary>
		/// The SQLite connection.
		/// </summary>
		readonly SQLiteConnection con;

		/// <summary>
		/// Initializes a new instance of the <see cref="TheSilentNet.Database"/> class.
		/// </summary>
		public Database () {

			// Make sure that nobody directly instantiates this class.
			// It must be instantiated from within the Instance () method.
			Guard ();

			// Create and open the connection.
			con = new SQLiteConnection {
				ConnectionString = string.Format ("Data Source={0}", DATA_SOURCE)
			}.OpenAndReturn ();

			// Create the initial table layout.
			// This does nothing if the tables already exist.
			ExecNonQuery (TABLE_LAYOUT);
		}

		/// <summary>
		/// Adds a node to the 'cips' table.
		/// </summary>
		/// <returns><c>true</c>, if the node was added, <c>false</c> otherwise.</returns>
		/// <param name="cip">cIP.</param>
		/// <param name="tln">Whether this node is a TLN.</param>
		public bool AddNode (string cip, bool tln = false) {
			return ExecNonQuery (tln ? QUERY_INSERT_TLN : QUERY_INSERT_SN, cip.ToSQLiteParam ("@cip")) > 0;
		}

		/// <summary>
		/// Adds a TLN to the 'cips' table.
		/// </summary>
		/// <returns><c>true</c>, if the TLN was added, <c>false</c> otherwise.</returns>
		/// <param name="cip">cIP.</param>
		public bool AddTopLevelNode (string cip) {
			return AddNode (cip: cip, tln: true);
		}

		/// <summary>
		/// Gets a maximum of <paramref name="max"/> nodes from the 'cips' table.
		/// </summary>
		/// <returns>The nodes.</returns>
		/// <param name="max">Node limit.</param>
		/// <param name="tln">Whether only TLNs should be returned.</param>
		public IEnumerable<CipEntry> GetNodes (int max = 1024, bool tln = false) {
			var nodes = new List<CipEntry> ();
			using (var reader = ExecReader ((tln ? QUERY_SELECT_TLN : QUERY_SELECT_KNOWN_SN).Limit (max))) {
				if (reader.HasRows)
					while (reader.Read ())
						nodes.Add (new CipEntry ((string)reader ["cip"], (int)reader ["tln"] == 1));
			}
			return nodes;
		}

		/// <summary>
		/// Gets a maximum of <paramref name="max"/> TLNs from the 'cips' table.
		/// </summary>
		/// <returns>The TLNs.</returns>
		/// <param name="max">Node limit.</param>
		public IEnumerable<CipEntry> GetTopLevelNodes (int max = 1024) {
			return GetNodes (max: 1024, tln: true);
		}

		/// <summary>
		/// Creates an SQLite command.
		/// </summary>
		/// <returns>The command.</returns>
		/// <param name="query">Query.</param>
		/// <param name="args">Arguments.</param>
		SQLiteCommand CreateCommand (string query, params KeyValuePair<string, object>[] args) {
			var com = con.CreateCommand ();
			com.CommandText = query;
			foreach (var kvp in args)
				com.Parameters.Add (new SQLiteParameter (kvp.Key, kvp.Value));
			com.Prepare ();
			return com;
		}

		/// <summary>
		/// Executes a query.
		/// </summary>
		/// <returns>The number of rows inserted/updated affected by the query.</returns>
		/// <param name="query">Query.</param>
		/// <param name="args">Arguments.</param>
		int ExecNonQuery (string query, params KeyValuePair<string, object>[] args) {
			return CreateCommand (query, args).ExecuteNonQuery ();
		}

		/// <summary>
		/// Executes a query.
		/// </summary>
		/// <returns>The first column of the resultset (if present), null otherwise.</returns>
		/// <param name="query">Query.</param>
		/// <param name="args">Arguments.</param>
		object ExecScalar (string query, params KeyValuePair<string, object>[] args) {
			return CreateCommand (query, args).ExecuteScalar ();
		}

		/// <summary>
		/// Executes a query.
		/// </summary>
		/// <returns>An SQLiteDataReader that can be used to access the resultset.</returns>
		/// <param name="query">Query.</param>
		/// <param name="args">Arguments.</param>
		SQLiteDataReader ExecReader (string query, params KeyValuePair<string, object>[] args) {
			return CreateCommand (query, args).ExecuteReader ();
		}

		#region IDisposable implementation

		/// <summary>
		/// Releases all resource used by the <see cref="TheSilentNet.Database"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="TheSilentNet.Database"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="TheSilentNet.Database"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="TheSilentNet.Database"/> so the garbage
		/// collector can reclaim the memory that the <see cref="TheSilentNet.Database"/> was occupying.</remarks>
		public void Dispose () {
			con.Close ();
			con.Shutdown ();
		}

		#endregion
	}
}

