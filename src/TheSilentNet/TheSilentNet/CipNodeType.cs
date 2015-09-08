using System;

namespace TheSilentNet
{
	/// <summary>
	/// cIP Node Type.
	/// </summary>
	[Flags]
	public enum CipNodeType {

		/// <summary>
		/// Describes a client that doesn't distribute data
		/// due to firewall restrictions or the choice of the user
		/// to not participate in the information exchange.
		/// </summary>
		AccessNode = 1 << 1,

		/// <summary>
		/// Describes a client that also distributes data.
		/// </summary>
		BottomNode = 1 << 2,

		/// <summary>
		/// Describes a top-level node.
		/// </summary>
		TopLevelNode = 1 << 3,
	}
}

