using System;

namespace TheSilentNet
{
	public abstract class Chainable<T> where T : class, new() {
		public static T GrabNew () {
			return new T ();
		}
	}
}

