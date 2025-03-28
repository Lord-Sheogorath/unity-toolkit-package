using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor
{
	public static class WindowUtil
	{
		public static T[] GetOpenWindows<T>()
			where T : EditorWindow
		{
			return Resources.FindObjectsOfTypeAll<T>();
		}
	}
}