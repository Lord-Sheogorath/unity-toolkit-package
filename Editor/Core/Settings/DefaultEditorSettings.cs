using UnityEngine;

namespace LordSheo.Editor
{
	public struct DefaultEditorSettings<T> : IDefaultEditorSettings<T>
		where T : EditorSettingsAsset
	{
		public T Create()
		{
			return ScriptableObject.CreateInstance<T>();
		}
	}
}