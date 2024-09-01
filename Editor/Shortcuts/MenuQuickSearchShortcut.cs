using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShortcutManagement;
using System.Linq;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

namespace Editor.Shortcuts
{
	public static class MenuQuickSearchShortcut
	{
		public const string NAMESPACE = "Custom/";

		public static List<string> menuNames = new List<string>()
		{
			"Assets",
			"Component",
			"Edit",
			"File",
			"GameObject",
			"Tools",
			"Window"
		};

		public static string[] selectedMenus;
		public static UnityEngine.Object[] selectedObjects;

		[InitializeOnLoadMethod]
		public static void Initialise()
		{
			EditorApplication.update += OnUpdate;
		}

		public static void OnUpdate()
		{
			if (selectedMenus == null || selectedMenus.Length == 0)
			{
				return;
			}

			Selection.objects = selectedObjects;

			foreach (var menu in selectedMenus)
			{
				EditorApplication.ExecuteMenuItem(menu);
			}

			selectedMenus = null;
			selectedObjects = null;
		}

		[Shortcut(NAMESPACE + "Menu/QuickSearch", KeyCode.Period, ShortcutModifiers.Control)]
		public static void OnShortcut()
		{
			selectedObjects = Selection.objects;

			var menus = menuNames
				.SelectMany(m => MenuProxy.GetMenuItems(m, true, true))
				.Where(m => m != null && Menu.GetEnabled(m.path))
				.Where(m => MenuProxy.MenuItemExists(m.path))

				// Remove open windows toolbar thingy
				.Where(m => m.path.StartsWith("Window/Panel") == false)
				.OrderBy(m => m.path)
				.ThenBy(m => m.priority);

			var items = menus
				.Select(m => new GenericSelectorItem<string>(m.path, m.path))
				.ToArray();

			var menu = new GenericSelector<string>(null, false, items);

			var window = menu.ShowInPopup(500);

			menu.SelectionConfirmed += OnSelectionConfirmed;
		}

		public static void OnSelectionConfirmed(IEnumerable<string> selection)
		{
			// Have to execute outside the scope of the popup
			selectedMenus = selection.ToArray();
		}
	}
}
