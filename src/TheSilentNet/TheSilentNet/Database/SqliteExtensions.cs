using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheSilentNet
{
    /// <summary>
    /// SQLite extensions.
    /// Static import for max efficiency.
    /// </summary>
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
        /// Boolean flags that control the behaviour
        /// of the query after executing it.
        /// </summary>
        public enum ExecFlags {
            PASS_QUERY = 1 << 1,
            CLEAR_QUERY = 1 << 2,
        }

        /// <summary>
        /// PRAGMA statement.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pragma"></param>
        /// <param name="value"></param>
        /// <param name="terminatestmt"></param>
        public static Query Pragma (this Query query, string pragma, object value, bool terminatestmt = true)
            => query.AppendFormat ("{0} PRAGMA {1} = {2}", terminatestmt ? ";" : string.Empty, pragma, value);

        /// <summary>
        /// PRAGMA statement.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pragma"></param>
        /// <param name="value"></param>
        /// <param name="terminatestmt"></param>
        public static Query Pragma (this Query query, SqliteNameReference pragma, SqliteNameReference value, bool terminatestmt = true)
            => query.AppendFormat ("{0} PRAGMA {1} = {2};", terminatestmt ? ";" : string.Empty, pragma, value);

        /// <summary>
        /// BEGIN statement.
        /// </summary>
        /// <param name="query"></param>
        public static Query BeginTransaction (this Query query)
            => query.AppendFormat ("BEGIN;");

        /// <summary>
        /// END statement.
        /// </summary>
        /// <param name="query"></param>
        public static Query EndTransaction (this Query query)
            => query.AppendFormat ("END;");

        /// <summary>
        /// FROM statement.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="table">Table.</param>
        public static Query From (this Query query, string table)
            => query.AppendFormat ("FROM {0}", table);

        /// <summary>
        /// WHERE statement.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="expr">Expression.</param>
        public static Query Where (this Query query, string expr)
            => query.AppendFormat ("WHERE {0}", expr);

        /// <summary>
        /// First part of an INSERT INTO statement.
        /// </summary>
        /// <param name="db">Database.</param>
        /// <param name="table">Table.</param>
        public static Query Into (this Query query, string table)
            => query.AppendFormat ("INSERT INTO {0}", table);

        /// <summary>
        /// Second part of an INSERT INTO statement.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="cols">Columns.</param>
        public static Query Insert (this Query query, params string[] cols)
            => query.AppendFormat ("({0})", cols.Aggregate (aggrstr));

        /// <summary>
        /// VALUES statement.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="cols">Columns.</param>
        public static Query Values (this Query query, params object[] cols)
            => query.AppendFormat ("VALUES ({0})", cols.Select (c => c.ToString ()).Aggregate (aggrstr));

        /// <summary>
        /// LIMIT statement.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="limit">Limit.</param>
        /// <param name="limit2">Limit2.</param>
        public static Query Limit (this Query query, int limit)
            => query.AppendFormat ("LIMIT {0}", limit);

        /// <summary>
        /// Checks if the last operand in the query string equals the specified operand.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="op">Operand.</param>
        public static Query Eq (this Query query, object op)
            => query.AppendFormat ("== {0}", op);

        /// <summary>
        /// Checks if the last operand in the query string doesn't equal the specified operand.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="op">Operand.</param>
        public static Query Neq (this Query query, object op)
            => query.AppendFormat ("!= {0}", op);

        /// <summary>
        /// AND statement.
        /// </summary>
        /// <param name="query">Query.</param>
        public static Query And (this Query query)
            => query.AppendFormat ("AND");

        /// <summary>
        /// OR statement.
        /// </summary>
        /// <param name="query">Query.</param>
        public static Query Or (this Query query)
            => query.AppendFormat ("OR");

        /// <summary>
        /// SELECT statement.
        /// </summary>
        /// <param name="db">Database.</param>
        /// <param name="cols">Columns.</param>
        public static Query Select (this Query query, params string[] cols) {
            return cols.Length == 1 ?
                query.AppendFormat ("SELECT {0}", cols.First ()) :
                query.AppendFormat ("SELECT ({0})", cols.Aggregate (aggrstr));
        }

        /// <summary>
        /// Adds an SQLite statement to the query and
        /// terminates the statement with a semicolon.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="statement"></param>
        public static Query State (this Query query, Func<Query, Query> statement) {
            var result = statement (query);
            return query.AppendFormat ("{0}{1}", result, result.ToString ().EndsWith (";") ? string.Empty : ";");
        }

        /// <summary>
        /// DROP statement, allows dropping of multiple tables.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="tables"></param>
        public static Query Drop (this Query query, params string[] tables) {
            foreach (var table in tables)
                query = query.AppendFormat ("DROP {0};", table);
            return query;
        }

        /// <summary>
        /// Maps an object to a KeyValuePair suitable to be used
        /// as a parameter for a prepared SQLite statement.
        /// </summary>
        /// <returns>The SQ lite parameter.</returns>
        /// <param name="value">Value.</param>
        /// <param name="key">Key.</param>
        public static KeyValuePair<string, object> ToSQLiteParam (this object value, string key)
            => new KeyValuePair<string, object> (key, value);
    }
}
