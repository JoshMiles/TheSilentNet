using System;
using System.Diagnostics.CodeAnalysis;

namespace TheSilentNet
{
	public abstract class Singleton<T> where T : new() {
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static readonly object syncRoot = new object ();
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		protected static T instance;
		public static T Instance () {
			// Analysis disable CompareNonConstrainedGenericWithNull
			if (instance == null)
				lock (syncRoot)
					if (instance == null)
						instance = new T ();
			// Analysis restore CompareNonConstrainedGenericWithNull
			return instance;
		}
	}
}

