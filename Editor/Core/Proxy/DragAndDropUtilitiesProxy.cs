#if LORD_SHEO_ODIN_ENABLED
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor
{
	public static class DragAndDropUtilitiesProxy
	{
		private static FieldInfo draggingObjectsField;

		static DragAndDropUtilitiesProxy()
		{
			var type = typeof(DragAndDropUtilities);
			
			draggingObjectsField = type
				.FindFieldInfo_Static("draggingObjects");
		}

		public static IEnumerable<object> GetCombinedDraggedObject()
		{
			// NOTE: Need to do this so we can combine the
			// UnityEngine.Object dragging list and raw
			// dragging list provided by Odin.
			return DragAndDrop.objectReferences
				.Cast<object>()
				.Concat(GetDraggedObjects());
		}
		/// <summary>
		/// Only returns non-UnityEngine.Object dragged objects
		/// </summary>
		public static object[] GetDraggedObjects()
		{
			return (object[])draggingObjectsField.GetValue(null);
		}
		public static void SetCombinedDraggedObjects(object[] baseObjectArr, UnityEngine.Object[] unityObjectArr)
		{
			draggingObjectsField.SetValue(null, baseObjectArr);
			DragAndDrop.objectReferences = unityObjectArr;
		}

		public static T[] DropZone<T>(Rect rect)
		{
			var draggingObjects = GetCombinedDraggedObject();

			var dropped = DragAndDropUtilities.DropZone<T>(rect, default, false);

			if (dropped != null)
			{
				return draggingObjects
					.OfType<T>()
					.ToArray();
			}

			return null;
		}
	}
}
#endif