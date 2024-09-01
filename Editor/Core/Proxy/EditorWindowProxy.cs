using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor
{
	public static class EditorWindowProxy
	{
		public static Type dockAreaType;
		public static Type editorWindowType;

		public static FieldInfo parentField;
		public static FieldInfo panesField;

		public static MethodInfo addTabMethod;
		public static MethodInfo removeTabMethod;

		static EditorWindowProxy()
		{
			dockAreaType = Type.GetType("UnityEditor.DockArea, UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
			editorWindowType = typeof(EditorWindow);

			parentField = editorWindowType.FindFieldInfo_Instance("m_Parent");
			panesField = dockAreaType.FindFieldInfo_Instance("m_Panes");

			addTabMethod = dockAreaType.FindMethodInfo_Instance("AddTab", typeof(EditorWindow), typeof(bool));
			removeTabMethod = dockAreaType.FindMethodInfo_Instance("RemoveTab", typeof(EditorWindow), typeof(bool), typeof(bool));
		}

		public static ScriptableObject GetParentContainer(EditorWindow window)
		{
			if (window == null)
			{
				return null;
			}

			return (ScriptableObject)parentField.GetValue(window);
		}
		public static EditorWindow[] GetContainerPanes(ScriptableObject container)
		{
			if (container == null)
			{
				return Array.Empty<EditorWindow>();
			}

			var panes = panesField.GetValue(container);

			if (panes == null)
			{
				return Array.Empty<EditorWindow>();
			}

			return ((IEnumerable<EditorWindow>)panes).ToArray();
		}

		public static void AddDockedWindow(ScriptableObject parent, EditorWindow window)
		{
			addTabMethod.Invoke(parent, new object[] { window, false });
		}
		public static void RemovedDockedWindow(ScriptableObject parent, EditorWindow window)
		{
			removeTabMethod.Invoke(parent, new object[] { window, true, false });
		}
	}
}
