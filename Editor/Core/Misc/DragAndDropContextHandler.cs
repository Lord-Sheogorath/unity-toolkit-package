using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor
{
	public static class DragAndDropContextHandler
	{
		public static readonly List<UnityEngine.Object> unityObjects = new();
		public static readonly List<object> objects = new();

		public static System.Action handlerCallback;
		
		public static bool isDirty;

		public static bool IsDragging => unityObjects.Count > 0 || objects.Count > 0;

		[InitializeOnLoadMethod]
		public static void Init()
		{
			EditorApplication.update -= Update;
			EditorApplication.update += Update;
		}

		private static void Update()
		{
			unityObjects.Clear();
			objects.Clear();

			if (DragAndDrop.objectReferences != null)
			{
				unityObjects.AddRange(DragAndDrop.objectReferences);
			}

			var tempObjects = DragAndDropUtilitiesProxy.GetDraggedObjects();

			if (tempObjects != null)
			{
				objects.AddRange(tempObjects);
			}

			try
			{
				handlerCallback?.Invoke();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			if (isDirty == false)
			{
				return;
			}

			try
			{
				DragAndDropUtilitiesProxy.SetCombinedDraggedObjects(objects.ToArray(), unityObjects.ToArray());
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
			finally
			{
				isDirty = false;
			}
		}

		public static void SetDirty()
		{
			isDirty = true;
		}
	}
}