﻿using System;
using System.Net;

namespace TheSilentNet
{
	/// <summary>
	/// cIP Entry.
	/// </summary>
	public class CipEntry
	{
        /// <summary>
        /// Shared WebClient.
        /// </summary>
        static readonly WebClient webclient;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static CipEntry () {
            webclient = new WebClient ();
            webclient.Proxy = new WebProxy ();
        }

        /// <summary>
        /// The cIP.
        /// </summary>
        public readonly string Value;

		/// <summary>
		/// The NodeType.
		/// </summary>
		public readonly CipNodeType Type;

		/// <summary>
		/// Gets a value indicating whether this instance is an accept node.
		/// </summary>
		/// <value><c>true</c> if this instance is an accept node; otherwise, <c>false</c>.</value>
		public bool IsAcceptNode { get { return Type.HasFlag (CipNodeType.AccessNode); } }

		/// <summary>
		/// Gets a value indicating whether this instance is a bottom node.
		/// </summary>
		/// <value><c>true</c> if this instance is a bottom node; otherwise, <c>false</c>.</value>
		public bool IsBottomNode { get { return Type.HasFlag (CipNodeType.BottomNode); } }

		/// <summary>
		/// Gets a value indicating whether this instance is a top level node.
		/// </summary>
		/// <value><c>true</c> if this instance is a top level node; otherwise, <c>false</c>.</value>
		public bool IsTopLevelNode { get { return Type.HasFlag (CipNodeType.TopLevelNode); } }

		/// <summary>
		/// Initializes a new instance of the <see cref="TheSilentNet.CipEntry"/> class.
		/// </summary>
		/// <param name="cip">cIP.</param>
		/// <param name="type">Type.</param>
		public CipEntry (string cip, CipNodeType type) {
			Value = cip;
			Type = type;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TheSilentNet.CipEntry"/> class.
		/// </summary>
		/// <param name="cip">cIP.</param>
		/// <param name="rawtype">Raw Type.</param>
		public CipEntry (string cip, int rawtype) : this (cip, (CipNodeType)rawtype) { }

        /// <summary>
        /// Generates a CipEntry from a given <see cref="EndPoint"/> and <see cref="CipNodeType"/>.
        /// </summary>
        /// <param name="ep"></param>
        /// <param name="type"></param>
        public static CipEntry GenerateFor (IPEndPoint ep, CipNodeType type = CipNodeType.AccessNode) {
            if (ep.Address == IPAddress.Any)
                return new CipEntry (webclient.DownloadString ("http://icanhazip.com").Trim ('\r', '\n'), type);
            return new CipEntry (ep.Address.ToString (), type);
        }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="TheSilentNet.CipEntry"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="TheSilentNet.CipEntry"/>.</returns>
		public override string ToString () {
			return string.Format ("[cIPNode: Accept={0}, Bottom={1}, TLN={2}, Value={3}]", IsAcceptNode, IsBottomNode, IsTopLevelNode, Value);
		}
	}
}

