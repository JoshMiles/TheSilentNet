using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheSilentNet {
    public static class DatabaseLayout {

        #region Premade queries

        /// <summary>
        /// Selects the cIPs of all TLNs from the 'cips' table.
        /// </summary>
        public static readonly Query QUERY_SELECT_TLN = "SELECT * FROM cips WHERE type == (1 << 3)";

        /// <summary>
        /// Selects the cIPs of all client nodes (excluding TLNs) from the 'cips' table.
        /// </summary>
        public static readonly Query QUERY_SELECT_CLN = "SELECT * FROM cips WHERE type != (1 << 3)";

        /// <summary>
        /// Inserts a cIP into the 'cips' table.
        /// </summary>
        public static readonly Query QUERY_INSERT_NODE = "INSERT INTO cips (cip, type) VALUES (@cip, @type)";

        #endregion

        /// <summary>
		/// The initial table layout.
		/// </summary>
		public const string TABLE_LAYOUT =

            // Create the table 'cips' only if it doesn't yet exist.
            "CREATE TABLE IF NOT EXISTS cips (" +

            // The cIP must not be null and is unique.
            // The query must fail on collision.
            "cip VARCHAR(512) NOT NULL UNIQUE ON CONFLICT FAIL," +

            // The type of the node.
            // Defaults to AcceptNode (2 ^ 1).
            "type TINYINT DEFAULT (1 << 1)" +
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
    }
}
