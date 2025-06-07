using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	public class TreeStyleProjectSettings : EditorSettingsAsset
	{
		public static TreeStyleProjectSettings Instance => EditorSettings.GetSettings<TreeStyleProjectSettings>();

		public bool deleteChildrenWithParent = true;
		public bool enableEditWithDoubleClick = true;
	}
}
