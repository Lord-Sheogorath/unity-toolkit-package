namespace LordSheo
{
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
