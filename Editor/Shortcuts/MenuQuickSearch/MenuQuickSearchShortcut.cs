using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShortcutManagement;
using System.Linq;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using System;
using LordSheo.Editor.UI;

namespace LordSheo.Editor.Shortcuts
{
	public static class MenuQuickSearchShortcut
	{
		public static readonly List<string> recentConfirmedMenus = new();
		
		public static string[] selectedMenus;
		public static UnityEngine.Object[] selectedObjects;

		public static MenuQuickSearchSettings Settings => MenuQuickSearchSettings.Instance;

		public static void OnNextUpdate()
		{
			if (selectedMenus == null || selectedMenus.Length == 0)
			{
				return;
			}

			var prevWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

			Selection.objects = selectedObjects;

			foreach (var menu in selectedMenus)
			{
				EditorApplication.ExecuteMenuItem(menu);
			}

			var newWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

			var openedWindow = newWindows
				.Except(prevWindows)
				.FirstOrDefault();

			EditorApplicationUtility.ForceFocusWindow(openedWindow);

			selectedMenus = null;
			selectedObjects = null;
		}

		[Shortcut(ConstValues.NAMESPACE_PATH + "Menu/QuickSearch", KeyCode.Period, ShortcutModifiers.Control)]
		public static void OnShortcut()
		{
			selectedObjects = Selection.objects;

			var hardMenuRefs = MenuProxy.cachedMenuRoots
				.SelectMany(m => MenuProxy.GetMenuItems(m, true, true))
				.Where(m => m != null && Menu.GetEnabled(m.path))
				.Where(m => MenuProxy.MenuItemExists(m.path))
				.ToList();
			
			// NOTE: Can cache this
			var menus = hardMenuRefs
				.Where(m => Settings.IsValid(m))

				// NOTE: Remove open windows toolbar thingy
				.Where(m => m.path.StartsWith("Window/Panel") == false)
				.OrderBy(m => m.path)
				.ThenBy(m => m.priority);

			var items = menus
				.Select(m => new GenericSelectorItem<StringSearch>(m.path, m.path))
				.ToList();

			if (recentConfirmedMenus.Count > 0)
			{
				for (int i = recentConfirmedMenus.Count - 1; i >= 0; i--)
				{
					var recentConfirmedMenu = recentConfirmedMenus[i];
					var recentConfirmMenuDisplayName = "Recent/" + recentConfirmedMenu.Split("/").Last();
					var recentConfirmedMenuItem = new GenericSelectorItem<StringSearch>(recentConfirmMenuDisplayName, recentConfirmedMenu);
					
					items.Insert(0, recentConfirmedMenuItem);
				}
			}

			var menu = new BetterGenericSelector<StringSearch>(null, false, items);
			menu.showItemPathsAsNames = Settings.showMenuPathsAsNames;
			
			var window = menu.ShowInPopup(500);

			menu.SetWindow(window);
			menu.SelectionConfirmed += OnSelectionConfirmed;
		}

		public static void OnSelectionConfirmed(IEnumerable<StringSearch> selection)
		{
			// NOTE: Have to execute outside the scope of the popup
			selectedMenus = selection.Select(s => s.value).ToArray();

			EditorApplicationUtility.AddNextUpdateCallback(OnNextUpdate);

			if (Settings.maxRecentMenuLength == 0)
			{
				recentConfirmedMenus.Clear();
				return;
			}
			
			foreach (var selected in selectedMenus)
			{
				recentConfirmedMenus.Remove(selected);
				recentConfirmedMenus.Insert(0, selected);
			}

			if (Settings.maxRecentMenuLength < 0)
			{
				return;
			}
			
			while (recentConfirmedMenus.Count > Settings.maxRecentMenuLength)
			{
				recentConfirmedMenus.RemoveAt(recentConfirmedMenus.Count - 1);
			}
		}
	}
}
