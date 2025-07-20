using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace LordSheo.Editor.GitSpoon
{
	public class GitSpoonManifestSourceSettings : EditorSettingsAsset 
	{
		[System.Serializable]
		public class SourceObject
		{
			public string name;
				
			[HideIf(nameof(IsRemoteUrl))]
			public TextAsset asset;
			[HideIf(nameof(IsAsset))]
			public string url;

			public string DisplayName
			{
				get
				{
					if (name.IsNullOrEmpty())
					{
						if (IsAsset)
						{
							return asset.name;
						}
						else if (IsRemoteUrl)
						{
							return url;
						}
						else
						{
							return "Unknown Source";
						}
					}
						
					return name;
				}
			}
				
			public bool IsAsset => asset != null;
			public bool IsRemoteUrl => url.IsNullOrEmpty() == false;
		}

		public static GitSpoonManifestSourceSettings Instance => EditorSettings.GetSettings<GitSpoonManifestSourceSettings>();

		[ListDrawerSettings(ListElementLabelName = nameof(SourceObject.DisplayName))]
		public List<SourceObject> sources = new();
	}
}