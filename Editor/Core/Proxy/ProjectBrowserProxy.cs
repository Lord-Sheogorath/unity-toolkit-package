using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	public static class ProjectBrowserProxy
	{
		public static Type type;

		public static FieldInfo lastInteractedProjectBrowserField;
		public static FieldInfo lastFoldersField;

		public static MethodInfo getFolderInstanceIdMethod;
		public static MethodInfo setFolderSelectionMethod;

		static ProjectBrowserProxy()
		{
			type = Type.GetType("UnityEditor.ProjectBrowser, UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

			lastInteractedProjectBrowserField = type.FindFieldInfo_Static("s_LastInteractedProjectBrowser");
			lastFoldersField = type.FindFieldInfo_Instance("m_LastFolders");

			getFolderInstanceIdMethod = type.FindMethodInfo_Static("GetFolderInstanceID", new Type[] { typeof(string) });
			setFolderSelectionMethod = type.FindMethodInfo_Instance("SetFolderSelection", new Type[] { typeof(int[]), typeof(bool) });
		}

		public static EditorWindow GetLastBrowser()
		{
			return (EditorWindow)lastInteractedProjectBrowserField.GetValue(null);
		}
		public static string[] GetLastFolders(EditorWindow browser)
		{
			return (string[])lastFoldersField.GetValue(browser);
		}

		public static int GetFolderInstanceID(string folder)
		{
			return (int)getFolderInstanceIdMethod.Invoke(null, new object[] { folder });
		}
		public static void SetFolderSelection(EditorWindow browser, int[] folderIds)
		{
			setFolderSelectionMethod.Invoke(browser, new object[] { folderIds, true });
		}
	}
}
