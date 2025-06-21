using System;
using System.Collections.Generic;
using UnityEngine;

namespace LordSheo
{
	public static class TransformExtensions
	{
		public static void Reset(this Transform source)
		{
			source.position = Vector3.zero;
			source.rotation = Quaternion.identity;
			source.localScale = Vector3.one;
		}
		
		public static void DestroyChildren(this Transform source)
		{
			var childCount = source.childCount;

			for (int i = childCount - 1; i >= 0; i--)
			{
				var child = source.GetChild(i);
				GameObject.Destroy(child.gameObject);
			}
		}
		public static IEnumerable<Transform> GetChildren(this Transform source)
		{
			for (int i = 0; i < source.childCount; i++)
			{
				yield return source.GetChild(i);
			}
		}
		public static void PerformOnChildren(this Transform source, System.Action<Transform> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}
			
			foreach (var child in source.GetChildren())
			{
				PerformOnChildren(child, action);
				action.Invoke(child);
			}
		}
	}
}