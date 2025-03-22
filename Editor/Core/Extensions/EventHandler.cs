using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor
{
	public static class EventHandler
	{
		public static EventType type;
		
		public static EventModifiers modifiers;
		public static KeyCode keyCode;
		public static int button;

		public static void Update(Event current)
		{
			try
			{
				if (current.type == EventType.KeyDown)
				{
					type = EventType.KeyDown;
				}
				else if (current.type == EventType.KeyUp)
				{
					type = EventType.KeyUp;
				}
				else if (current.keyCode == KeyCode.None)
				{
					type = EventType.Ignore;
				}
				
				modifiers = current.modifiers;
				keyCode = current.keyCode;
				button = current.button;
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

		public static void Use()
		{
			type = EventType.Used;
		}
	}
}
