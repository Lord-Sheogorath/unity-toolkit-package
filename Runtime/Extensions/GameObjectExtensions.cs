using UnityEngine;

namespace LordSheo
{
	public static class GameObjectExtensions
	{
		public static T GetOrAddComponent<T>(this GameObject source)
			where T : Component
		{
			if (source.TryGetComponent<T>(out var comp) == false)
			{
				comp = source.AddComponent<T>();
			}

			return comp;
		}

		public static bool IsNull(this UnityEngine.Object obj)
		{
			return obj == null || obj.Equals(null);
		}
	}
}