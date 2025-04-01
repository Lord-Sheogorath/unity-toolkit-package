using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Shortcuts
{
	public class MenuQuickSearchSettings
	{
		private struct DefaultSettings : IDefaultEditorSettings<MenuQuickSearchSettings>
		{
			public MenuQuickSearchSettings Create()
			{
				return new MenuQuickSearchSettings();
			}
		}
		public static MenuQuickSearchSettings Instance => EditorSettings.GetSettings<MenuQuickSearchSettings>(new DefaultSettings());

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

		[SettingsProvider]
		private static SettingsProvider GetSettings()
		{
			return EditorSettingsProvider<MenuQuickSearchSettings>.Create(new DefaultSettings());
		}
	}
}
