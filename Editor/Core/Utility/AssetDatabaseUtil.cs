using System.Data;
using System.IO;
using UnityEditor;

namespace LordSheo.Editor
{
	public static class AssetDatabaseUtil
	{
		public static void CreateDirectory(string path)
		{
			if (Directory.Exists(path))
			{
				return;
			}
			
			Directory.CreateDirectory(path);
			
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}