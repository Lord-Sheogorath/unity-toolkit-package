using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UEditorPrefs = UnityEditor.EditorPrefs;

namespace LordSheo.Editor
{
	public class EditorPrefs : IPrefs
	{
		public static EditorPrefs Instance { get; set; } = new EditorPrefs();

		public bool Contains(string key)
		{
			return UEditorPrefs.HasKey(key);
		}

		public void Delete(string key)
		{
			UEditorPrefs.DeleteKey(key);
		}

		public void Save()
		{
		}

		public bool GetBool(string key, bool defaultVal = false)
		{
			return UEditorPrefs.GetBool(Format(key), defaultVal);
		}

		public float GetFloat(string key, float defaultVal = 0)
		{
			return UEditorPrefs.GetFloat(Format(key), defaultVal);
		}

		public int GetInt(string key, int defaultVal = 0)
		{
			return UEditorPrefs.GetInt(Format(key), defaultVal);
		}

		public string GetString(string key, string defaultVal = null)
		{
			return UEditorPrefs.GetString(Format(key), defaultVal);
		}

		public void SetBool(string key, bool value)
		{
			UEditorPrefs.SetBool(Format(key), value);
		}

		public void SetFloat(string key, float value)
		{
			UEditorPrefs.SetFloat(Format(key), value);
		}

		public void SetInt(string key, int value)
		{
			UEditorPrefs.SetInt(Format(key), value);
		}

		public void SetString(string key, string value)
		{
			UEditorPrefs.SetString(Format(key), value);
		}

		public string Format(string key)
		{
			return Application.productName + "_" + key;
		}
	}
}
