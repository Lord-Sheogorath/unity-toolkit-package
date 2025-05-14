using UnityEditor;

namespace LordSheo.Editor
{
	public class ProjectEditorPrefs : EditorPrefs
	{
		public override string Format(string key)
		{
			return PlayerSettings.productGUID + "_" + key;
		}
	}
}
