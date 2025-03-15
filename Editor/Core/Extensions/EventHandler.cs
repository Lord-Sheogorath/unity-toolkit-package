using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor
{
	public static class EventHandler
	{
		public static EventModifiers modifiers;
		public static KeyCode keyCode;
		public static EventType type;

		public static void Update(Event current)
		{
			try
			{
				var valid = current.type switch
				{
					EventType.KeyDown => true,
					EventType.KeyUp => true,

					_ => false,
				};

				if (valid == false)
				{
					return;
				}

				modifiers = current.modifiers;
				keyCode = current.keyCode;
				type = current.type;
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static bool IsKeyDown(KeyCode key)
		{
			return key == keyCode && type == EventType.KeyDown;
		}
		public static bool IsKeyUp(KeyCode key)
		{
			return key == keyCode && type == EventType.KeyUp;
		}
	}
}
