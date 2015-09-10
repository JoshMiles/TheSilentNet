using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheSilentNet
{
    public static class SqliteExtensions {
        public static readonly SqliteNameReference
            // Booleans
            ON = "ON",
            OFF = "OFF",
            // Pragmas
            SYNCHRONOUS = "synchronous",
            JOURNAL_MODE = "journal_mode",
            FOREIGN_KEYS = "foreign_keys",
            // Journal modes
            JOURNAL_MODE_WAL = "WAL",
            JOURNAL_MODE_DELETE = "DELETE";

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

        public enum ExecFlags {
            PASS_QUERY = 1 << 1,
            CLEAR_QUERY = 1 << 2,
        }

        public static Query Pragma (this Query query, string pragma, object value, bool terminatestmt = true) {
            return query.AppendFormat ("{0} PRAGMA {1} = {2}", terminatestmt ? ";" : string.Empty, pragma, value);
        }

        public static Query Pragma (this Query query, SqliteNameReference pragma, SqliteNameReference value, bool terminatestmt = true) {
            return query.AppendFormat ("{0} PRAGMA {1} = {2};", terminatestmt ? ";" : string.Empty, pragma, value);
        }

        public static Query BeginTransaction (this Query query) {
            return query.AppendFormat ("BEGIN;");
        }

        public static Query EndTransaction (this Query query) {
            return query.AppendFormat ("END;");
        }

        public static Query State (this Query query, Func<Query, Query> statement) {
            var result = statement (query);
            return query.AppendFormat ("{0}{1}", result, result.ToString ().EndsWith (";") ? string.Empty : ";");
        }

        public static Query Drop (this Query query, params string[] tables) {
            foreach (var table in tables)
                query = query.AppendFormat ("DROP {0};", table);
            return query;
        }

        /// <summary>
        /// SELECT statement.
        /// </summary>
        /// <param name="db">Database.</param>
        /// <param name="cols">Columns.</param>
        public static Query Select (this Query query, params string[] cols) {
            return cols.Length == 0 ?
                query.AppendFormat ("SELECT ()") : cols.Length == 1 ?
                query.AppendFormat ("SELECT {0}", cols.First ()) :
                query.AppendFormat ("SELECT ({0})", cols.Aggregate (aggrstr));
        }

        /// <summary>
        /// FROM statement.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="table">Table.</param>
        public static Query From (this Query query, string table) {
            return query.AppendFormat ("FROM {0}", table);
        }

        /// <summary>
        /// WHERE statement.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="expr">Expression.</param>
        public static Query Where (this Query query, string expr) {
            return query.AppendFormat ("WHERE {0}", expr);
        }

        /// <summary>
        /// WHERE statement.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="op1">First operand.</param>
        /// <param name="comp">Type of comparison.</param>
        /// <param name="op2">Second operand.</param>
        public static Query Where (this Query query, string op1, WhereComparison comp, string op2) {
            return query.AppendFormat ("WHERE {0} {1} {2}", op1, oplookup[comp], op2);
        }

        /// <summary>
        /// First part of an INSERT INTO statement.
        /// </summary>
        /// <param name="db">Database.</param>
        /// <param name="table">Table.</param>
        public static Query Into (this Query query, string table) {
            return query.AppendFormat ("INSERT INTO {0}", table);
        }

        /// <summary>
        /// Second part of an INSERT INTO statement.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="cols">Columns.</param>
        public static Query Insert (this Query query, params string[] cols) {
            return query.AppendFormat ("({0})", cols.Aggregate (aggrstr));
        }

        /// <summary>
        /// VALUES statement.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="cols">Columns.</param>
        public static Query Values (this Query query, params object[] cols) {
            return query.AppendFormat ("VALUES ({0})", cols.Select (c => c.ToString ()).Aggregate (aggrstr));
        }

        /// <summary>
        /// LIMIT statement.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="limit">Limit.</param>
        /// <param name="limit2">Limit2.</param>
        public static Query Limit (this Query query, int limit, int limit2 = -1) {
            return limit2 == -1 ?
                query.AppendFormat ("LIMIT {0}", limit) :
                query.AppendFormat ("LIMIT {0},{1}", limit, limit2);
        }

        /// <summary>
        /// Checks if the last operand in the query string equals the specified operand.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="op">Operand.</param>
        public static Query Eq (this Query query, object op) {
            return query.AppendFormat ("== {0}", op);
        }

        /// <summary>
        /// Checks if the last operand in the query string doesn't equal the specified operand.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="op">Operand.</param>
        public static Query Neq (this Query query, object op) {
            return query.AppendFormat ("!= {0}", op);
        }

        /// <summary>
        /// AND statement.
        /// </summary>
        /// <param name="query">Query.</param>
        public static Query And (this Query query) {
            return query.AppendFormat ("AND");
        }

        /// <summary>
        /// OR statement.
        /// </summary>
        /// <param name="query">Query.</param>
        public static Query Or (this Query query) {
            return query.AppendFormat ("OR");
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
