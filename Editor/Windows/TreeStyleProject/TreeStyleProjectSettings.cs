using UnityEditor;

namespace LordSheo.Editor.Windows
{
	public class TreeStyleProjectSettings
	{
		private struct DefaultSettings : IDefaultEditorSettings<TreeStyleProjectSettings>
		{
			public TreeStyleProjectSettings Create()
			{
				return new();
			}
		}

		public static TreeStyleProjectSettings Instance => EditorSettings.GetSettings(new DefaultSettings());

		public bool deleteChildrenWithParent = true;
		public bool enableEditWithDoubleClick = true;

		[SettingsProvider]
		private static SettingsProvider GetSettings()
		{
			return EditorSettingsProvider<TreeStyleProjectSettings>.Create(new DefaultSettings());
		}
	}
}
