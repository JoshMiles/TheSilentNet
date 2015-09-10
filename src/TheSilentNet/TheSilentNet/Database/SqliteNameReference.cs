using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheSilentNet {
    public class SqliteNameReference {
        string value;

        public SqliteNameReference (string val) {
            value = val;
        }

        public static implicit operator SqliteNameReference (string str) => new SqliteNameReference (str);
        public override string ToString () => value;
    }
}
