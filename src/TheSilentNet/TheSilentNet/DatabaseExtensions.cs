using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSilentNet
{
	/// <summary>
	/// Database extensions.
	/// </summary>
	public static class DatabaseExtensions {

		/// <summary>
		/// An anonymous function that concatenates strings.
		/// </summary>
		/// <example>
		/// Input:	"Hello", "World"
		/// Output:	"Hello, World"
		/// </example>
		readonly static Func<string, string, string> aggrstr = (p1, p2) => string.Format ("{0}, {1}", p1, p2);

		/// <summary>
		/// A lookup table containing the string equivalent of a specific <see cref="WhereComparison"/>. 
		/// </summary>
		readonly static Dictionary<WhereComparison, string> oplookup = new Dictionary<WhereComparison, string> {
			{ WhereComparison.Equals, "==" },
			{ WhereComparison.NotEquals, "!=" },
		};

		/// <summary>
		/// A comparison used in a WHERE statement.
		/// </summary>
		public enum WhereComparison {
			Equals,
			NotEquals,
		}

		/// <summary>
		/// SELECT statement.
		/// </summary>
		/// <param name="db">Database.</param>
		/// <param name="cols">Columns.</param>
		public static string Select (this Database db, params string[] cols) {
			return cols.Length == 0 ?
				string.Format ("SELECT ()") : cols.Length == 1 ?
				string.Format ("SELECT {0}", cols.First ()) :
				string.Format ("SELECT ({0})", cols.Aggregate (aggrstr));
		}

		/// <summary>
		/// FROM statement.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="table">Table.</param>
		public static string From (this string query, string table) {
			return string.Format ("{0} FROM {1}", query, table);
		}

		/// <summary>
		/// WHERE statement.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="expr">Expression.</param>
		public static string Where (this string query, string expr) {
			return string.Format ("{0} WHERE {1}", query, expr);
		}

		/// <summary>
		/// WHERE statement.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="op1">First operand.</param>
		/// <param name="comp">Type of comparison.</param>
		/// <param name="op2">Second operand.</param>
		public static string Where (this string query, string op1, WhereComparison comp, string op2) {
			return string.Format ("{0} WHERE {1} {2} {3}", query, op1, oplookup [comp], op2);
		}

		/// <summary>
		/// First part of an INSERT INTO statement.
		/// </summary>
		/// <param name="db">Database.</param>
		/// <param name="table">Table.</param>
		public static string Into (this Database db, string table) {
			return string.Format ("INSERT INTO {0}", table);
		}

		/// <summary>
		/// Second part of an INSERT INTO statement.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="cols">Columns.</param>
		public static string Insert (this string query, params string[] cols) {
			return string.Format ("{0} ({1})", query, cols.Aggregate (aggrstr));
		}

		/// <summary>
		/// VALUES statement.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="cols">Columns.</param>
		public static string Values (this string query, params object[] cols) {
			return string.Format ("{0} VALUES ({1})", query, cols
				.Select (c => c.ToString ()).Aggregate (aggrstr));
		}

		/// <summary>
		/// LIMIT statement.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="limit2">Limit2.</param>
		public static string Limit (this string query, int limit, int limit2 = -1) {
			return limit2 == -1 ?
				string.Format ("{0} LIMIT {1}", query, limit) :
				string.Format ("{0} LIMIT {1},{2}", query, limit, limit2);
		}

		/// <summary>
		/// Checks if the last operand in the query string equals the specified operand.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="op">Operand.</param>
		public static string Eq (this string query, object op) {
			return string.Format ("{0} == {1}", query, op);
		}

		/// <summary>
		/// Checks if the last operand in the query string doesn't equal the specified operand.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="op">Operand.</param>
		public static string Neq (this string query, object op) {
			return string.Format ("{0} != {1}", query, op);
		}

		/// <summary>
		/// AND statement.
		/// </summary>
		/// <param name="query">Query.</param>
		public static string And (this string query) {
			return string.Format ("{0} AND", query);
		}

		/// <summary>
		/// OR statement.
		/// </summary>
		/// <param name="query">Query.</param>
		public static string Or (this string query) {
			return string.Format ("{0} OR", query);
		}

		/// <summary>
		/// Maps an object to a KeyValuePair suitable to be used
		/// as a parameter for a prepared SQLite statement.
		/// </summary>
		/// <returns>The SQ lite parameter.</returns>
		/// <param name="value">Value.</param>
		/// <param name="key">Key.</param>
		public static KeyValuePair<string, object> ToSQLiteParam (this object value, string key) {
			return new KeyValuePair<string, object> (key, value);
		}
	}
}

