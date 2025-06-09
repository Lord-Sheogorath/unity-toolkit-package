namespace LordSheo
{
	public interface IReadOnlyPrefs
	{
		int GetInt(string key, int defaultVal = 0);
		float GetFloat(string key, float defaultVal = 0);
		bool GetBool(string key, bool defaultVal = false);
		string GetString(string key, string defaultVal = null);

		bool Contains(string key);
	}
}