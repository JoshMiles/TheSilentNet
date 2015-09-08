using System;
using System.Diagnostics.CodeAnalysis;

namespace TheSilentNet
{
	/// <summary>
	/// Singleton.
	/// </summary>
	public abstract class Singleton<T> where T : new() {
		
		/// <summary>
		/// The sync root.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		static readonly object syncRoot = new object ();

		/// <summary>
		/// The instance.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		protected static T instance;

		/// <summary>
		/// Lazily creates and returns the instance.
		/// </summary>
		public static T Instance () {
			// Analysis disable CompareNonConstrainedGenericWithNull
			if (instance == null)
				lock (syncRoot)
					if (instance == null)
						instance = new T ();
			// Analysis restore CompareNonConstrainedGenericWithNull
			return instance;
		}

		/// <summary>
		/// Protects the constructor from being called directly.
		/// </summary>
		public static void Guard () {
			// Analysis disable once CompareNonConstrainedGenericWithNull
			if (instance != null)
				throw new Exception ("You must instantiate this class using the Instance () method.");
		}
	}
}

