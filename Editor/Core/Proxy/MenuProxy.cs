using System.Linq;
using UnityEditor;
using System.Reflection;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace LordSheo.Editor
{
	public static class MenuProxy
	{
		public static MethodInfo getMenuItemsMethod;
		public static MethodInfo getMenuItemDefaultShortcutsMethod;

		public static MethodInfo removeMenuItemMethod;
		public static MethodInfo addMenuItemMethod;

		public static MethodInfo menuItemExistsMethod;

		public readonly static string[] cachedMenuItems;
		public readonly static string[] cachedMenuRoots;

		static MenuProxy()
		{
			var type = typeof(Menu);

			getMenuItemsMethod = type
				.FindMethodInfo_Static("GetMenuItems");

			getMenuItemDefaultShortcutsMethod = type
				.FindMethodInfo_Static("GetMenuItemDefaultShortcuts");

			removeMenuItemMethod = type
				.FindMethodInfo_Static("RemoveMenuItem");

			addMenuItemMethod = type
				.FindMethodInfo_Static("AddMenuItem");

			menuItemExistsMethod = type
				.FindMethodInfo_Static("MenuItemExists");

			try
			{
				var menus = new List<string>();
				GetMenuItemDefaultShortcuts(menus, new());

				cachedMenuItems = menus.ToArray();
				cachedMenuRoots = cachedMenuItems
					.Select(i => i.Split("/", StringSplitOptions.RemoveEmptyEntries))
					.Select(s => s.FirstOrDefault())
					.Where(n => string.IsNullOrEmpty(n) == false)
					.Distinct()
					.ToArray();

			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static ScriptingMenuItemProxy[] GetMenuItems(string menuPath, bool includeSeparators, bool localized)
		{
			var result = (Array)getMenuItemsMethod
				.Invoke(null, new object[] { menuPath, includeSeparators, localized });

			var menus = new ScriptingMenuItemProxy[result.Length];

			for (int i = 0; i < result.Length; i++)
			{
				var item = result.GetValue(i);
				menus[i] = new ScriptingMenuItemProxy(item);
			}

			return menus;
		}

		public static void GetMenuItemDefaultShortcuts(List<string> outItemNames, List<string> outItemDefaultShortcuts)
		{
			getMenuItemDefaultShortcutsMethod.Invoke(null, new object[] { outItemNames, outItemDefaultShortcuts });
		}

		public static void RemoveMenuItem(string menuItem)
		{
			removeMenuItemMethod.Invoke(null, new object[] { menuItem });
		}
		public static void AddMenuItem(string name, string shortcut, bool isChecked, int priority, System.Action action, Func<bool> validate)
		{
			addMenuItemMethod.Invoke(null, new object[] { name, shortcut, isChecked, priority, action, validate });
		}

		public static bool MenuItemExists(string name)
		{
			return (bool)menuItemExistsMethod.Invoke(null, new object[] { name });
		}
	}
}
