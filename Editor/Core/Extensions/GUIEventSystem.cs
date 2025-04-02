using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor
{
	public class GUIEventSystem
	{
		public class EventValue
		{
			public EventType type;
			public EventModifiers modifiers;
			public KeyCode keyCode;
			public int button;
			
			public EventValue(Event current)
			{
				type = current.type;
				modifiers = current.modifiers;
				keyCode = current.keyCode;
				button = current.button;
			}

			public void Use()
			{
				type = EventType.Used;
			}
		}

		public static GUIEventSystem Current { get; } = new();
		
		public EventValue value;
		
		public event System.Action<EventValue> OnInputCallback;
		
		public void Update()
		{
			var value = new EventValue(Event.current);
			
			switch (Event.current.type)
			{
				case EventType.KeyDown:
				case EventType.KeyUp:
				case EventType.MouseDown:
				case EventType.MouseUp:
					this.value = value;
					OnInputCallback?.Invoke(value);
				break;
			}
		}
	}
}
