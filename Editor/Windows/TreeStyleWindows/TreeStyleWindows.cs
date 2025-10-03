#if LORD_SHEO_ODIN_ENABLED
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Windows
{
	public class TreeStyleWindows : OdinMenuEditorWindow
	{
		[MenuItem("Custom/Windows/" + nameof(TreeStyleWindows))]
		public static TreeStyleWindows Open()
		{
			return EditorWindow.CreateWindow<TreeStyleWindows>(nameof(TreeStyleWindows));
		}

		protected override OdinMenuTree BuildMenuTree()
		{
			var tree = new OdinMenuTree();

			foreach (var window in FindOpenWindowsOfType<EditorWindow>())
			{
				tree.Add(window.titleContent.text, window);
			}

			tree.Selection.SelectionChanged += OnSelectionChanged;

			return tree;
		}

		private void OnSelectionChanged(SelectionChangedType type)
		{
			if (type != SelectionChangedType.ItemAdded)
			{
				return;
			}

			var window = (EditorWindow)MenuTree.Selection.SelectedValue;
			window.ShowTab();
		}

		public static T[] FindOpenWindowsOfType<T>()
			where T : EditorWindow
		{
			return Resources.FindObjectsOfTypeAll<T>();
		}
	}
}
#endif