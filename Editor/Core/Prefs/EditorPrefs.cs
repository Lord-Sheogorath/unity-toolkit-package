namespace LordSheo.Editor
{
	public class EditorPrefs : IPrefs
	{
		public bool Contains(string key)
		{
			return UnityEditor.EditorPrefs.HasKey(key);
		}

		public void Delete(string key)
		{
			UnityEditor.EditorPrefs.DeleteKey(key);
		}

		public void Save()
		{
		}

		public bool GetBool(string key, bool defaultVal = false)
		{
			return UnityEditor.EditorPrefs.GetBool(Format(key), defaultVal);
		}

		public float GetFloat(string key, float defaultVal = 0)
		{
			return UnityEditor.EditorPrefs.GetFloat(Format(key), defaultVal);
		}

		public int GetInt(string key, int defaultVal = 0)
		{
			return UnityEditor.EditorPrefs.GetInt(Format(key), defaultVal);
		}

		public string GetString(string key, string defaultVal = null)
		{
			return UnityEditor.EditorPrefs.GetString(Format(key), defaultVal);
		}

		public void SetBool(string key, bool value)
		{
			UnityEditor.EditorPrefs.SetBool(Format(key), value);
		}

		public void SetFloat(string key, float value)
		{
			UnityEditor.EditorPrefs.SetFloat(Format(key), value);
		}

		public void SetInt(string key, int value)
		{
			UnityEditor.EditorPrefs.SetInt(Format(key), value);
		}

		public void SetString(string key, string value)
		{
			UnityEditor.EditorPrefs.SetString(Format(key), value);
		}

		public virtual string Format(string key)
		{
			return key;
		}	
	}
}