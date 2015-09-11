using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace TheSilentNet
{
	/// <summary>
	/// Singleton.
	/// </summary>
	public static class Singleton {
		public readonly static Dictionary<string, object> Lookup = new Dictionary<string, object> ();
        public readonly static List<Type> List = new List<Type> ();
		public static readonly object SyncRoot = new object ();
	}

	/// <summary>
	/// Singleton.
	/// </summary>
	public abstract class Singleton<T> where T : class, new() {

		/// <summary>
		/// Lazily creates and returns the instance.
		/// </summary>
		public static T Instance () {
			if (!Singleton.List.Contains (typeof (T)))
				lock (Singleton.SyncRoot)
					if (!Singleton.List.Contains (typeof (T))) {
                        Singleton.List.Add (typeof (T));
						Singleton.Lookup.Add (typeof (T).ToString (), null);
                        Singleton.Lookup[typeof (T).ToString ()] = new T ();
                    }
			return Singleton.Lookup[typeof (T).ToString ()] as T;
		}

		/// <summary>
		/// Protects the constructor from being called directly.
		/// </summary>
		public static void Guard () {
			if (!Singleton.List.Contains (typeof (T)))
				throw new Exception ("You must instantiate this class using the Instance () method.");
		}
	}
}

