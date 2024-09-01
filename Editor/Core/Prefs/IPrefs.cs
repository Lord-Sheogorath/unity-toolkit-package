namespace LordSheo.Editor
{
	public interface IReadOnlyPrefs
	{
		int GetInt(string key, int defaultVal = 0);
		float GetFloat(string key, float defaultVal = 0);
		bool GetBool(string key, bool defaultVal = false);
		string GetString(string key, string defaultVal = null);

		bool Contains(string key);
	}

	public interface IPrefs : IReadOnlyPrefs
    {
        void SetInt(string key, int value);
        void SetFloat(string key, float value);
        void SetBool(string key, bool value);
        void SetString(string key, string value);

        void Save();
        void Delete(string key);
    }
}
