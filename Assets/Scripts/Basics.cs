// Simple, general-purpose utility functions and extension methods go here

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class B {
	public static void Assert(bool condition, string message = "something went wrong") {
		if (!condition) {
			throw new UnityException("Assertion failed - " + message);
			// MonoDevelop is weird and inconsistent about intercepting Exceptions, so might wanna put a breakpoint here
		}
	}
}

public static class ExtensionMethods {
	public static bool AllUnique<T>(this List<T> list)
	{
		return new HashSet<T>(list).Count == list.Count;
	}
	
	/// <summary>Shallow copy</summary>
	public static List<T> Copy<T>(this List<T> list)
	{
		return new List<T>(list);
	}
	
	/// <summary>Shallow copy</summary>
	public static HashSet<T> Copy<T>(this HashSet<T> set)
	{
		return new HashSet<T>(set);
	}

	/// <summary>Retrieve value from dictionary, or default if not found.</summary>
	public static V Get<K, V>(this Dictionary<K, V> dictionary, K key, V defaultValue = default(V)) {
		V result;
		if (dictionary.TryGetValue(key, out result))
			return result;
		return defaultValue;
	}

	/// <summary>Return a random permutation of this list.</summary>
	public static List<T> Shuffled<T>(this IEnumerable<T> list) {  
		var rng = new System.Random();  
		
		return list.OrderBy(GUIElement => rng.Next()).ToList();
	}
}

