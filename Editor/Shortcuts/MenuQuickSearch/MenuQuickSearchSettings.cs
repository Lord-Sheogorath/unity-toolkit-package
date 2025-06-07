using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Shortcuts
{
	public class MenuQuickSearchSettings : EditorSettingsAsset
	{
		public static MenuQuickSearchSettings Instance => EditorSettings.GetSettings<MenuQuickSearchSettings>();

		public bool showMenuPathsAsNames = false;
		public List<StringMatch> ignoredMenuMatches = new();

		public bool IsValid(ScriptingMenuItemProxy item)
		{
			foreach (var match in ignoredMenuMatches)
			{
				if (match.IsMatch(item.path))
				{
					return false;
				}
			}

			return true;
		}
	}
}
