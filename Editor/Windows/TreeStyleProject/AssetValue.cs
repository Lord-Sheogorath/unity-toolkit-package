using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	public class AssetValue : IValue
	{
		public string guid;
		public string name;

		[JsonIgnore]
		public UnityEngine.Object asset;

		[JsonIgnore]
		public string Name => IsValid() ? name : guid;

		[JsonIgnore]
		public Texture Icon { get; set; }

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
				Icon = null;
			}

			var path = AssetDatabase.GUIDToAssetPath(guid);

			name = path.Split("/").LastOrDefault();
			asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

			Icon = AssetDatabase.GetCachedIcon(path);
		}
		public void Select()
		{
		}
		public void OnGUI(Rect rect)
		{
			if (Event.current.IsKeyDown(KeyCode.LeftControl))
			{
				Debug.Log("LEFT CTRL IS DOWN");
				var color = Color.cyan;
				color.a = 0.25f;

				EditorGUI.DrawRect(rect, color);
				DragAndDropUtilities.DragZone<UnityEngine.Object>(rect, asset, true, true);
			}

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
				Selection.activeObject = asset;
				EditorGUIUtility.PingObject(asset);
			}
			else if (Event.current.clickCount == 2)
			{
				if (TreeStyleProjectSettings.Instance.enableEditWithDoubleClick)
				{
					AssetDatabase.OpenAsset(asset);
				}
			}
		}
	}
}