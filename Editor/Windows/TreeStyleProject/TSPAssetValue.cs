#if LORD_SHEO_ODIN_ENABLED
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	[TypeGuid("3F409B29-2BE5-45CF-8B0C-DB129EFAE616")]
	public class TSPAssetValue : ITreeStyleValue
	{
		[JsonIgnore]
		public TreeStyleProjectSettings Settings => TreeStyleProjectSettings.Instance;
		
		public string guid;

		[JsonIgnore]
		public string name;

		[JsonIgnore]
		public string path;

		[JsonIgnore]
		public UnityEngine.Object asset;

		[JsonIgnore]
		public string Name => IsValid() ? name : guid;

		[JsonIgnore]
		public Texture Icon => GetCurrentIcon();

		public event Action ModifiedCallback;
		
		private Texture assetIcon;

		public bool IsValid()
		{
			return string.IsNullOrEmpty(guid) == false
				&& asset != null;
		}

		public void Refresh()
		{
			if (IsValid() == false)
			{
				asset = null;
				assetIcon = null;
			}

			path = AssetDatabase.GUIDToAssetPath(guid);
			name = path.Split("/").LastOrDefault();
			asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

			assetIcon = AssetDatabase.GetCachedIcon(path);
		}

		public void Select()
		{
		}

		public void Deselect()
		{
		}

		public void OnGUI(Rect rect)
		{
			if (rect.Contains(Event.current.mousePosition) == false)
			{
				return;
			}

			// Make sure they're left clicking
			if (Event.current.button != 0)
			{
				return;
			}

			if (Event.current.clickCount == 1)
			{
				SingleClick();
			}
			else if (Event.current.clickCount == 2)
			{
				DoubleClick();
			}
		}
		
		private void SingleClick()
		{
			if (Event.current.type != EventType.MouseUp)
			{
				return;
			}
			
			if (Event.current.control)
			{
				var selection = Selection.objects
					.ToHashSet();

				if (asset != null)
				{
					var added = selection.Add(asset);

					if (added == false)
					{
						selection.Remove(asset);
					}

					Selection.objects = selection.ToArray();
				}

				Event.current.Use();
			}
			else
			{
				Selection.activeObject = asset;
			}

			EditorGUIUtility.PingObject(asset);
		}
		private void DoubleClick()
		{
			if (Settings.enableEditWithDoubleClick == false)
			{
				return;
			}

			AssetDatabase.OpenAsset(asset);
		}

		public Texture GetCurrentIcon()
		{
			if (Selection.objects.Any(o => o == asset))
			{
				return EditorIcons.Link.Active;
			}

			return assetIcon;
		}

		public IEnumerable<GenericSelectorItem<System.Action>> GetContextActions()
		{
			yield return new("Open", () => { AssetDatabase.OpenAsset(asset); });
		}
	}
}
#endif