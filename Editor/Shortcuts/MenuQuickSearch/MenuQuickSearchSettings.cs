using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LordSheo.Editor.Shortcuts
{
	public class MenuQuickSearchSettings : EditorSettingsAsset
	{
		public static MenuQuickSearchSettings Instance => EditorSettings.GetSettings<MenuQuickSearchSettings>();

		public bool showMenuPathsAsNames = false;
		public int maxRecentMenuLength = 10;
		public bool persistRecentMenuSelections = true;
		[Tooltip("Assets created with MenuQuickSearch may serialise incorrectly, force an asset database save & refresh")]
		public bool forceAssetSaveAndRefreshAfterConfirm = true;
		public List<StringMatch> ignoredMenuMatches = new();
		public List<StringMatch> requiredConfirmationMenuMatches = new();
		
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
