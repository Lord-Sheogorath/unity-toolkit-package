namespace LordSheo.Editor
{
	public interface IDefaultEditorSettings<T>
		where T : EditorSettingsAsset
	{
		public T Create();
	}
}