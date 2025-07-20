using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor
{
	public static class EditorApplicationUtility
	{
		public static readonly List<System.Action> onNextUpdateCallbacks = new();

		[InitializeOnLoadMethod]
		public static void Initialise()
		{
			EditorApplication.update += OnUpdate;
		}

		public static void OnUpdate()
		{
			InvokeOnNextUpdateCallbacks();
		}

		public static void InvokeOnNextUpdateCallbacks()
		{
			if (onNextUpdateCallbacks == null || onNextUpdateCallbacks.Count == 0)
			{
				return;
			}

			// NOTE: In case one of the callbacks wants to do something
			// on the next update we don't want to break it.
			var callbacks = onNextUpdateCallbacks.ToArray();

			onNextUpdateCallbacks.Clear();

			foreach (var callback in callbacks)
			{
				try
				{
					callback?.Invoke();
				}
				catch (System.Exception e)
				{
					Debug.LogException(e);
				}
			}
		}

		public static void AddNextUpdateCallback(System.Action action)
		{
			onNextUpdateCallbacks.Add(action);
		}
		public static void AddNextUpdateCallback(System.Action action, int count)
		{
			if (count <= 0)
			{
				return;
			}

			onNextUpdateCallbacks.Add(() =>
			{
				action?.Invoke();
				AddNextUpdateCallback(action, count - 1);
			});
		}

		public static void ForceFocusWindow(EditorWindow window)
		{
			if (window == null)
			{
				return;
			}

			// NOTE: Need to wait until inspectors have updated
			// before the window is valid for focusing
			EditorApplication.delayCall += () =>
			{
				if (window == null)
				{
					return;
				}

				window.Show();
				window.Focus();
			};
		}

		public static void DisplayConfirmAction(string title, string text, System.Action action)
		{
			var confirmed = EditorUtility.DisplayDialog(title, text, "Confirm", "Cancel");

			if (confirmed == false)
			{
				return;
			}
			
			action?.Invoke();
		}

		public static bool IsWindowOpen<T>()
			where T : EditorWindow
		{
			return Resources.FindObjectsOfTypeAll<EditorWindow>()
				.OfType<T>()
				.Any();
		}
	}
}
