using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShortcutManagement;
using System.Linq;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using System;
using LordSheo.Editor.UI;
using Newtonsoft.Json;

namespace LordSheo.Editor.Shortcuts
{
	public static class MenuQuickSearchShortcut
	{
		private const string KEY_RECENT_MENUS = "mqs_recent";
		
		public static readonly List<string> recentMenuSelections = new();
		
		public static string[] selectedMenus;
		public static UnityEngine.Object[] selectedObjects;

		private static bool initialised = false;

		public static MenuQuickSearchSettings Settings => MenuQuickSearchSettings.Instance;

		public static void OnNextUpdate()
		{
			if (selectedMenus == null || selectedMenus.Length == 0)
			{
				return;
			}

			try
			{
				var prevWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

				Selection.objects = selectedObjects;

				foreach (var menu in selectedMenus)
				{
					if (Settings.requiredConfirmationMenuMatches.Any(m => m.IsMatch(menu)))
					{
						EditorApplicationUtility.DisplayConfirmAction("MQS - Confirm Action", menu, () =>
						{
							EditorApplication.ExecuteMenuItem(menu);
						});
					}
					else
					{
						EditorApplication.ExecuteMenuItem(menu);
					}
				}

				var newWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

				var openedWindow = newWindows
					.Except(prevWindows)
					.FirstOrDefault();

				EditorApplicationUtility.ForceFocusWindow(openedWindow);

				selectedMenus = null;
				selectedObjects = null;
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			
			if (Settings.forceAssetSaveAndRefreshAfterConfirm)
			{
				// FIX: Potential fix for created assets losing references
				// to other assets due to weird serialisation issues.
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}

		[Shortcut(ConstValues.NAMESPACE_PATH + "Menu/QuickSearch", KeyCode.Period, ShortcutModifiers.Control)]
		public static void OnShortcut()
		{
			if (initialised == false)
			{
				initialised = true;

				if (Settings.persistRecentMenuSelections)
				{
					var recentConfirmedMenuJson = EditorProject.prefs.GetString(KEY_RECENT_MENUS);

					if (recentConfirmedMenuJson.IsNullOrEmpty() == false)
					{
						recentMenuSelections.AddRange(JsonConvert.DeserializeObject<List<string>>(recentConfirmedMenuJson));
					}
				}
			}
			
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

			if (recentMenuSelections.Count > 0)
			{
				for (int i = recentMenuSelections.Count - 1; i >= 0; i--)
				{
					var recentConfirmedMenu = recentMenuSelections[i];
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

		[MenuItem(ConstValues.NAMESPACE_PATH + "/MenuQuickSearch/Clear Recent Menu Selections")]
		public static void ClearRecentMenuSelections()
		{
			recentMenuSelections.Clear();
			SaveRecentMenuSelections();
		}
		
		public static void OnSelectionConfirmed(IEnumerable<StringSearch> selection)
		{
			// NOTE: Have to execute outside the scope of the popup
			selectedMenus = selection.Select(s => s.value).ToArray();

			EditorApplicationUtility.AddNextUpdateCallback(OnNextUpdate);

			if (Settings.maxRecentMenuLength == 0)
			{
				recentMenuSelections.Clear();
				return;
			}
			
			foreach (var selected in selectedMenus)
			{
				recentMenuSelections.Remove(selected);
				recentMenuSelections.Insert(0, selected);
			}

			// If less than zero, allow unlimited entries.
			if (Settings.maxRecentMenuLength > 0)
			{
				while (recentMenuSelections.Count > Settings.maxRecentMenuLength)
				{
					recentMenuSelections.RemoveAt(recentMenuSelections.Count - 1);
				}
			}

			SaveRecentMenuSelections();
		}

		private static void SaveRecentMenuSelections()
		{
			if (Settings.persistRecentMenuSelections)
			{
				EditorProject.prefs.SetString(KEY_RECENT_MENUS, JsonConvert.SerializeObject(recentMenuSelections));
				EditorProject.prefs.Save();
			}
		}
	}
}
