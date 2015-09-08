using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSilentNet
{
	public static class DatabaseExtensions {
		readonly static Func<string, string, string> aggrstr = (p1, p2) => string.Format ("{0}, {1}", p1, p2);
		readonly static Dictionary<WhereOperation, string> oplookup = new Dictionary<WhereOperation, string> {
			{ WhereOperation.Equals, "==" }
		};
		public enum WhereOperation {
			Equals = 1 << 1,
		}
		public static string Select (this Database db, params string[] what) {
			return what.Length == 0 ?
				string.Format ("SELECT ()") : what.Length == 1 ?
				string.Format ("SELECT {0}", what.First ()) :
				string.Format ("SELECT ({0})", what.Aggregate (aggrstr));
		}

		public static string From (this string query, string table) {
			return string.Format ("{0} FROM {1}", query, table);
		}

		public static string Where (this string query, string expr) {
			return string.Format ("{0} WHERE {1}", query, expr);
		}

		public static string Where (this string query, string col, WhereOperation op, string comp) {
			return string.Format ("{0} WHERE {1} {2} {3}", query, col, oplookup [op], comp);
		}

		public static string Into (this Database db, string table) {
			return string.Format ("INSERT INTO {0}", table);
		}

		public static string Insert (this string query, params string[] cols) {
			return string.Format ("{0} ({1})", query, cols.Aggregate (aggrstr));
		}

		public static string Values (this string query, params object[] cols) {
			return string.Format ("{0} ({1})", query, cols
				.Select (c => c.ToString ()).Aggregate (aggrstr));
		}

		public static string Eq (this string query, object comp) {
			return string.Format ("{0} == {1}", query, comp);
		}

		public static string Limit (this string query, int limit, int limit2 = -1) {
			return limit2 == -1 ?
				string.Format ("{0} LIMIT {1}", query, limit) :
				string.Format ("{0} LIMIT {1},{2}", query, limit, limit2);
		}

		public static KeyValuePair<string, object> ToSQLiteParam (this object value, string key) {
			return new KeyValuePair<string, object> (key, value);
		}
	}
}

