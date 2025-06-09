using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor
{
	public static class EditorSettings
	{
		public static readonly Dictionary<Type, EditorSettingsAsset> settings = new();

		public static T GetSettings<T>(IDefaultEditorSettings<T> fallback = null)
			where T : EditorSettingsAsset
		{
			if (fallback == null)
			{
				fallback = new DefaultEditorSettings<T>();
			}
			
			var type = typeof(T);
			var name = type.Name;

			if (EditorSettings.settings.TryGetValue(type, out var value))
			{
				if (value != null)
				{
					return (T)value;
				}
				
				Debug.LogError("NullSettings: " + name);
			}

			var asset = Resources.Load<T>("LordSheo/Settings/" + name);

			if (asset == null)
			{
				var path = "Assets/Resources/LordSheo/Settings";
				AssetDatabaseUtil.CreateDirectory(path);
				
				asset = fallback.Create();
				
				AssetDatabase.CreateAsset(asset, $"{path}/{name}.asset");
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			EditorSettings.settings[type] = asset;
			
			return asset;
		}

		public static void SetSettings<T>(T settings)
			where T : EditorSettingsAsset
		{
			try
			{
				var type = typeof(T);
				EditorSettings.settings[type] = settings;
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}