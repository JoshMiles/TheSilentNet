using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.Linq;

namespace TheSilentNet
{
	public class Database : Singleton<Database>, IDisposable
	{
		const string DATA_SOURCE = "cipc.db";
		const string TABLE_LAYOUT =
			"PRAGMA foreign_keys = ON;" +
			"CREATE TABLE IF NOT EXISTS cips (" +
			"cip VARCHAR(512) NOT NULL UNIQUE ON CONFLICT FAIL," +
			"tln BLOB(1) DEFAULT 0" +
			"); CREATE TABLE IF NOT EXISTS domains (" +
			"cip VARCHAR(512) NOT NULL UNIQUE ON CONFLICT FAIL," +
			"name VARCHAR(64) NOT NULL UNIQUE ON CONFLICT FAIL," +
			"port INT DEFAULT 80," +
			"FOREIGN KEY(cip) REFERENCES cips(cip)" +
			");";

		readonly string QUERY_SELECT_TLN = Instance ().Select ("cip").From ("cips").Where ("tln").Eq (1).Limit (1024);
		readonly string QUERY_SELECT_KNOWN_SN = Instance ().Select ("cip").From ("cips").Where ("tln").Eq (0).Limit (1024);
		readonly string QUERY_INSERT_TLN = Instance ().Into ("cip").Insert ("cip", "tln").Values ("@cip", 1);
		readonly string QUERY_INSERT_SN = Instance ().Into ("cip").Insert ("cip").Values ("@cip");

		readonly SQLiteConnection con;

		public Database () {
			if (instance != null)
				throw new Exception ("You must instantiate this class using the Instance () method.");
			con = new SQLiteConnection {
				ConnectionString = string.Format ("Data Source={0}", DATA_SOURCE)
			}.OpenAndReturn ();
			ExecNonQuery (TABLE_LAYOUT);
		}

		SQLiteCommand CreateCommand (string query, params KeyValuePair<string, object>[] args) {
			var com = con.CreateCommand ();
			com.CommandText = query;
			foreach (var kvp in args)
				com.Parameters.Add (new SQLiteParameter (kvp.Key, kvp.Value));
			com.Prepare ();
			return com;
		}

		void ExecNonQuery (string query, params KeyValuePair<string, object>[] args) {
			CreateCommand (query, args).ExecuteNonQuery ();
		}

		object ExecScalar (string query, params KeyValuePair<string, object>[] args) {
			return CreateCommand (query, args).ExecuteScalar ();
		}

		SQLiteDataReader ExecReader (string query, params KeyValuePair<string, object>[] args) {
			return CreateCommand (query, args).ExecuteReader ();
		}

		#region IDisposable implementation

		public void Dispose () {
			con.Close ();
			con.Shutdown ();
		}

		#endregion
	}
}

