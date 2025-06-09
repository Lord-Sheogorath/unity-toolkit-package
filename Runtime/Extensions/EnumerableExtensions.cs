using System;
using System.Collections.Generic;

namespace LordSheo
{
	public static class EnumerableExtensions
	{
		public static bool IsNullOrEmpty<T>(this ICollection<T> source)
		{
			return source == null || source.Count == 0;
		}

		public static void Perform<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
			{
				action.Invoke(item);
			}
		}

		public static TValue GetOrAddDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue = default)
		{
			if (source.TryGetValue(key, out var value) == false)
			{
				source[key] = value = defaultValue;
			}

			return value;
		}
	}
}