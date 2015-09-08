using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TheSilentNet
{
	/// <summary>
	/// Singleton.
	/// </summary>
	public static class Singleton {
		public readonly static Dictionary<Type, object> Lookup = new Dictionary<Type, object> ();
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
			if (!Singleton.Lookup.ContainsKey (typeof(T)))
				lock (Singleton.SyncRoot)
					if (!Singleton.Lookup.ContainsKey (typeof(T))) {
						Singleton.Lookup.Add (typeof(T), null);
						Singleton.Lookup [typeof(T)] = new T ();
					}
			return (T)Singleton.Lookup[typeof (T)];
		}

		/// <summary>
		/// Protects the constructor from being called directly.
		/// </summary>
		public static void Guard () {
			if (!Singleton.Lookup.ContainsKey (typeof (T)))
				throw new Exception ("You must instantiate this class using the Instance () method.");
		}
	}
}

