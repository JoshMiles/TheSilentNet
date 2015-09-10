using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheSilentNet {
    public class Query : Chainable<Query> {
        string value;

        public Query () {
            value = string.Empty;
        }

        public Query (string querystr) {
            value = querystr;
        }

        public Query Exec (params KeyValuePair<string, object>[] args) {
            Database.Instance ().ExecNonQuery (value, args);
            return this;
        }

        public Query Exec (SqliteExtensions.ExecFlags flags, params KeyValuePair<string, object>[] args) {
            Database.Instance ().ExecNonQuery (value, args);
            switch (flags) {
                case SqliteExtensions.ExecFlags.PASS_QUERY:
                    return this;
                case SqliteExtensions.ExecFlags.CLEAR_QUERY:
                    return Clear ();
            }
            return this;
        }

        public Query Invoke (Action act) {
            act ();
            return this;
        }

        public Query Clear () {
            value = string.Empty;
            return this;
        }

        public int ExecNonQuery (params KeyValuePair<string, object>[] args) {
            return Database.Instance ().ExecNonQuery (value, args);
        }

        public Query Append (string query) {
            value += query;
            return this;
        }

        public Query AppendFormat (string format, params object[] args) {
            value = string.Format ("{0}{1}", value, string.Format (format, args));
            return this;
        }

        public static implicit operator string (Query query) => query.ToString ();
        public static implicit operator Query (string query) => new Query (query);
        public override string ToString () => value;
    }
}
