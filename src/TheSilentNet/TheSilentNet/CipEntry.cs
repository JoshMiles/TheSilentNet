using System;

namespace TheSilentNet
{
	/// <summary>
	/// cIP Entry.
	/// </summary>
	public class CipEntry
	{
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
		/// Gets a value indicating whether this instance is a client node.
		/// </summary>
		/// <value><c>true</c> if this instance is a client node; otherwise, <c>false</c>.</value>
		public bool IsClientNode { get { return Type.HasFlag (CipNodeType.ClientNode); } }

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
		public CipEntry (string cip, int rawtype) : this (cip, (CipNodeType)rawtype) {
		}
	}
}

