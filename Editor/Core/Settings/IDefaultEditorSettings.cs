namespace LordSheo.Editor
{
	/// <summary>
	/// Use this to create and initialise an instance of your settings object.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IDefaultEditorSettings<T>
		where T : EditorSettingsAsset
	{
		public T Create();
	}
}