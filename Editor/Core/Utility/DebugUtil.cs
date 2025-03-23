using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace LordSheo.Editor
{
	public static class DebugUtil
	{
		public static void LogElapsedMilliseconds(string name, System.Action action)
		{
			var stopwatch = new Stopwatch();
			
			try
			{
				stopwatch.Start();
				
				action?.Invoke();
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
			finally
			{
				Debug.Log($"Elapsed Milliseconds ({name}): {stopwatch.ElapsedMilliseconds}ms");
				stopwatch.Stop();
			}
		}
	}
}