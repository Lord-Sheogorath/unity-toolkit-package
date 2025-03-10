using System.Collections.Generic;
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

		public List<string> menuNames = new List<string>()
		{
			"Assets",
			"Component",
			"Edit",
			"File",
			"GameObject",
			"Tools",
			"Window"
		};

		[SettingsProvider]
		private static SettingsProvider GetSettings()
		{
			return EditorSettingsProvider<MenuQuickSearchSettings>.Create(new DefaultSettings());
		}
	}
}
