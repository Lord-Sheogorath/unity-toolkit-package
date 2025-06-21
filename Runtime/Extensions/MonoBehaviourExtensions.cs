using System.Collections;
using UnityEngine;

namespace LordSheo
{
	public static class MonoBehaviourExtensions
	{
		public static Coroutine PerformAfterSeconds(this MonoBehaviour source, System.Action action, float seconds)
		{
			return source.StartCoroutine(Perform());

			IEnumerator Perform()
			{
				yield return new WaitForSeconds(seconds);
				action?.Invoke();
			}
		}
		public static Coroutine PerformAfterSecondsRealtime(this MonoBehaviour source, System.Action action, float seconds)
		{
			return source.StartCoroutine(Perform());

			IEnumerator Perform()
			{
				yield return new WaitForSecondsRealtime(seconds);
				action?.Invoke();
			}
		}

		public static Coroutine PerformAtEndOfFrame(this MonoBehaviour source, System.Action action)
		{
			return source.StartCoroutine(Perform());

			IEnumerator Perform()
			{
				yield return new WaitForEndOfFrame();
				action?.Invoke();
			}
		}
		public static Coroutine PerformOnNextFrame(this MonoBehaviour source, System.Action action)
		{
			return source.StartCoroutine(Perform());

			IEnumerator Perform()
			{
				yield return null;
				action?.Invoke();
			}
		}

		public static Coroutine PerformAfterComplete(this MonoBehaviour source, System.Action action, System.Func<bool> predicate)
		{
			return source.StartCoroutine(Perform());

			IEnumerator Perform()
			{
				yield return new WaitUntil(predicate);
				action?.Invoke();
			}
		}
	}
}