using System.Collections.Generic;
using System.Linq;
using UnityEditor;

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

		public List<string> ignoredMenuNames = new List<string>();

		public bool IsValid(ScriptingMenuItemProxy item)
		{
			return ignoredMenuNames.Any(n => item.path.StartsWith(n)) == false;
		}

		[SettingsProvider]
		private static SettingsProvider GetSettings()
		{
			return EditorSettingsProvider<MenuQuickSearchSettings>.Create(new DefaultSettings());
		}
	}
}
