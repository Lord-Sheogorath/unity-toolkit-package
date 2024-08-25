using UnityEditor;
using System.Reflection;
using System;
using UnityEngine;

namespace Editor
{
	public class ScriptingMenuItemProxy
	{
		public static Type type;

		public static readonly PropertyInfo pathProperty;
		public static readonly PropertyInfo isSeparatorProperty;
		public static readonly PropertyInfo priorityProperty;

		public readonly string path;

		public readonly bool isSeparator;
		public readonly int priority;

		static ScriptingMenuItemProxy()
		{
			type = Type.GetType("UnityEditor.ScriptingMenuItem, UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

			pathProperty = type.GetProperty("path");
			isSeparatorProperty = type.GetProperty("isSeparator");
			priorityProperty = type.GetProperty("priority");
		}

		public ScriptingMenuItemProxy(object item)
		{
			if (item.GetType() != type)
			{
				throw new System.ArgumentException($"Invalid type: {item.GetType()}", nameof(item));
			}

			path = (string)pathProperty.GetValue(item);
			isSeparator = (bool)isSeparatorProperty.GetValue(item);
			priority = (int)priorityProperty.GetValue(item);
		}

		public ScriptingMenuItemProxy(string path, bool isSeparator = false, int priority = -1)
		{
			this.path = path;
			this.isSeparator = isSeparator;
			this.priority = priority;
		}
	}
}
