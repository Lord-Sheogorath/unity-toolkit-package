using UnityEngine;
using UnityEditor.ShortcutManagement;
using UnityEditor;
using System;

namespace LordSheo.Editor.Shortcuts
{
	public static class WindowCloseShortcut
	{
		public const string NAMESPACE = "Custom/";

		public static Type type;

		public static string title;
		public static EditorWindow parent;

		[Shortcut(ConstValues.NAMESPACE_PATH + "Window/Close", KeyCode.W, ShortcutModifiers.Control)]
		public static void OnShortcut()
		{
			var window = EditorWindow.focusedWindow;

			if (window == null)
			{
				return;
			}

			var confirmed = EditorUtility.DisplayDialog($"Close WIndow", $"Are you sure you want to close '{window.titleContent}'?", "Confirm", "Cancel");

			if (confirmed == false)
			{
				return;
			}

			try
			{
				type = window.GetType();
				title = window.titleContent.text;
				parent = null; // TO-DO: Add support

				window.Close();
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}
