using UnityEngine;

namespace LordSheo.Editor
{
	public static class EventExtensions
	{
		public static bool IsKeyboardEvent(this Event current)
		{
			return current.type is EventType.KeyDown 
				or EventType.KeyUp;
		}
		public static bool IsMouseEvent(this Event current)
		{
			return current.type is EventType.MouseDown
				or EventType.MouseUp
				or EventType.MouseMove
				or EventType.MouseDrag
				or EventType.DragPerform
				or EventType.DragUpdated
				or EventType.DragExited;
		}
		
		public static bool IsKeyDown(this Event current, KeyCode key)
		{
			return current.keyCode == key && current.type != EventType.KeyUp;
		}
		public static bool IsKeyUp(this Event current, KeyCode key)
		{
			return current.keyCode == key && current.type == EventType.KeyUp;
		}

		public static bool IsMouseDown(this Event current, int button)
		{
			return current.button == button && current.type == EventType.MouseDown;
		}

		public static bool IsMouseUp(this Event current, int button)
		{
			return current.button == button && current.type == EventType.MouseUp;
		}
	}
}