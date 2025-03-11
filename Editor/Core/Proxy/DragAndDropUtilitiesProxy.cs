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

		public static IEnumerable<object> GetDraggingObjects()
		{
			// NOTE: Need to do this so we can combine the
			// UnityEngine.Object dragging list and raw
			// dragging list provided by Odin.
			var refs = DragAndDrop.objectReferences.Cast<object>();

			var objects = (object[])draggingObjectsField.GetValue(null);

			return refs.Concat(objects);
		}
		public static T[] DropZone<T>(Rect rect)
		{
			var draggingObjects = GetDraggingObjects();

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
