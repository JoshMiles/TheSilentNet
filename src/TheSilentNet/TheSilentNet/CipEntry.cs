using System;

namespace TheSilentNet
{
	public class CipEntry
	{
		readonly string cip;
		readonly bool tln;

		public string Value { get { return cip; } }
		public bool IsTopLevel { get { return tln; } }

		public CipEntry (string cip, bool tln) {
			this.cip = cip;
			this.tln = tln;
		}
	}
}

