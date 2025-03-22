using UnityEngine;

namespace LordSheo.Editor
{
	public static class EventExtensions
	{
		public static bool IsKeyDown(this Event current, KeyCode key)
		{
			return current.keyCode == key && current.type != EventType.KeyUp;
		}
		public static bool IsKeyUp(this Event current, KeyCode key)
		{
			return current.keyCode == key && current.type == EventType.KeyUp;
		}
	}
}