using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor
{
	public interface IDefaultEditorSettings<T>
	{
		public T Create();
	}

	public static class EditorSettings
	{
		public static readonly JsonSerializerSettings jsonSettings = new()
		{
			// WARNING: Fixes an issue when deserialising already
			// populated lists. Without setting this the default
			// behaviour is the merge the existing values with the
			// the new values.
			ObjectCreationHandling = ObjectCreationHandling.Replace,
		};

		public static readonly Dictionary<Type, object> settings = new();

		public static T GetSettings<T>(IDefaultEditorSettings<T> fallback)
		{
			var type = typeof(T);

			if (EditorSettings.settings.TryGetValue(type, out var value))
			{
				return (T)value;
			}

			var json = UnityEditor.EditorPrefs.GetString(type.Name, string.Empty);
			var settings = default(T);
			var valid = false;

			if (string.IsNullOrEmpty(json) == false)
			{
				try
				{
					settings = DeserialiseSettings<T>(json);
					valid = true;
				}
				catch (System.Exception e)
				{
					Debug.LogException(e);
				}
			}

			if (valid == false)
			{
				settings = fallback.Create();
			}

			EditorSettings.settings[type] = settings;

			return settings;
		}
		public static string GetSerialisedSettings<T>(IDefaultEditorSettings<T> fallback)
		{
			return SerialiseSettings(GetSettings(fallback));
		}

		public static void SetSettings<T>(T settings)
		{
			try
			{
				var type = typeof(T);
				EditorSettings.settings[type] = settings;

				var json = SerialiseSettings(settings);
				UnityEditor.EditorPrefs.SetString(type.Name, json);
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
		}
		public static void SetSerialisedSettings<T>(string json)
		{
			try
			{
				var settings = JsonConvert.DeserializeObject<T>(json, jsonSettings);
				SetSettings(settings);
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static T DeserialiseSettings<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json, jsonSettings);
		}
		public static string SerialiseSettings<T>(T settings)
		{
			return JsonConvert.SerializeObject(settings, jsonSettings);
		}
	}
}