using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using static TheSilentNet.DatabaseLayout;
using static TheSilentNet.SqliteExtensions;
using static TheSilentNet.SqliteExtensions.ExecFlags;

namespace TheSilentNet {
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
            // This does nothing if the tables already exists.
            Query.GrabNew ()
                .Pragma (FOREIGN_KEYS, ON)
                .Append (TABLE_LAYOUT)
                .Exec ();
		}

		/// <summary>
		/// Adds a node to the 'cips' table.
		/// </summary>
		/// <returns><c>true</c>, if the node was added, <c>false</c> otherwise.</returns>
		/// <param name="cip">cIP.</param>
		/// <param name="type">Type.</param>
		public bool AddNode (string cip, CipNodeType type) {
            var pcip = cip.ToSQLiteParam ("@cip");
            var ptype = ((int)type).ToSQLiteParam ("@type");
            return QUERY_INSERT_NODE.ExecNonQuery (pcip, ptype) > 0;
		}

		/// <summary>
		/// Adds a node to the 'cips' table.
		/// </summary>
		/// <returns><c>true</c>, if the node was added, <c>false</c> otherwise.</returns>
		/// <param name="cip">cIP entry.</param>
		public bool AddNode (CipEntry cip) => AddNode (cip.Value, cip.Type);

        /// <summary>
		/// Wraps a function call into a transaction.
		/// </summary>
		/// <param name="act">Action.</param>
		public void WrapTransaction (Action act) => Query.GrabNew ()
            .BeginTransaction ()
            .Exec (CLEAR_QUERY)
            .Invoke (act)
            .EndTransaction ()
            .Exec ();

        /// <summary>
        /// Wraps a function call into a fast transaction.
        /// Possibly unsafe. Don't use this in production.
        /// </summary>
        /// <param name="act">Act.</param>
        public void WrapFast (Action act) => Query.GrabNew ()
            .Pragma (SYNCHRONOUS, OFF)
            .Pragma (JOURNAL_MODE, JOURNAL_MODE_WAL)
            .BeginTransaction ()
            .Exec (CLEAR_QUERY)
            .Invoke (act)
            .EndTransaction ()
            .Pragma (JOURNAL_MODE, JOURNAL_MODE_DELETE)
            .Pragma (SYNCHRONOUS, ON)
            .Exec ();

        /// <summary>
        /// Recreates the database.
        /// WARNING: Wipes all data!
        /// </summary>
        public bool Recreatedb () => Query.GrabNew ()
            .Drop ("cips", "domains")
            .Pragma (FOREIGN_KEYS, ON)
            .Append (TABLE_LAYOUT)
            .ExecNonQuery () > 0;

		/// <summary>
		/// Gets a maximum of <paramref name="max"/> nodes from the 'cips' table.
		/// </summary>
		/// <returns>The nodes.</returns>
		/// <param name="max">Node limit.</param>
		/// <param name="tln">Whether only TLNs should be returned.</param>
		public IEnumerable<CipEntry> GetNodes (int max = 1024, bool tln = false) {
			var nodes = new List<CipEntry> ();
			using (var reader = ExecReader ((tln ? QUERY_SELECT_TLN : QUERY_SELECT_CLN).Limit (max))) {
				if (reader.HasRows)
					while (reader.Read ())
						nodes.Add (new CipEntry (reader.GetString (0), reader.GetByte (1)));
			}
			return nodes;
		}

        /// <summary>
        /// Gets a maximum of <paramref name="max"/> TLNs from the 'cips' table.
        /// </summary>
        /// <returns>The TLNs.</returns>
        /// <param name="max">Node limit.</param>
        public IEnumerable<CipEntry> GetTopLevelNodes (int max = 1024) => GetNodes (max: 1024, tln: true);

        /// <summary>
        /// Creates an SQLite command.
        /// </summary>
        /// <returns>The command.</returns>
        /// <param name="query">Query.</param>
        /// <param name="args">Arguments.</param>
        [SuppressMessage ("Microsoft.Security", "CA2100")]
        public SQLiteCommand CreateCommand (string query, params KeyValuePair<string, object>[] args) {
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
		public int ExecNonQuery (string query, params KeyValuePair<string, object>[] args) {
			return CreateCommand (query, args).ExecuteNonQuery ();
		}

		/// <summary>
		/// Executes a query.
		/// </summary>
		/// <returns>The first column of the resultset (if present), null otherwise.</returns>
		/// <param name="query">Query.</param>
		/// <param name="args">Arguments.</param>
		public object ExecScalar (string query, params KeyValuePair<string, object>[] args) {
			return CreateCommand (query, args).ExecuteScalar ();
		}

		/// <summary>
		/// Executes a query.
		/// </summary>
		/// <returns>An SQLiteDataReader that can be used to access the resultset.</returns>
		/// <param name="query">Query.</param>
		/// <param name="args">Arguments.</param>
		public SQLiteDataReader ExecReader (string query, params KeyValuePair<string, object>[] args) {
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
            Dispose (true);
		}

        protected virtual void Dispose (bool dispose) {
            if (dispose) {
                con.Close ();
                con.Shutdown ();
            }
        }

		#endregion
	}
}

