using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;

namespace LordSheo.Editor.Windows.TSP
{
	// NOTE: We want to modify the dragged list when moving between the TreeStyleProjectWindow
	// and any other window. Since we don't care about Nodes outside the TSPWindow we want
	// to instead use their asset reference if they have one.
	public static class DragAndDropWindowContext
	{
		private static readonly Dictionary<UnityEngine.Object, Node<IValue>> draggedNodeReferences = new();
		
		[InitializeOnLoadMethod]
		public static void Init()
		{
			DragAndDropContextHandler.handlerCallback += Update;
		}

		private static void Update()
		{
			var window = EditorWindow.mouseOverWindow;

			if (window == null)
			{
				return;
			}

			if (DragAndDropContextHandler.IsDragging == false)
			{
				draggedNodeReferences.Clear();
			}

			var draggedObjects = DragAndDropContextHandler.objects;
			var draggedUnityObjects = DragAndDropContextHandler.unityObjects;

			if (window is not TreeStyleProjectWindow)
			{
				for (var index = draggedObjects.Count - 1; index >= 0; index--)
				{
					var obj = draggedObjects[index];
					var node = obj as Node<IValue>;

					if (node == null)
					{
						continue;
					}

					var assetValue = node.value as AssetValue;

					if (assetValue == null || assetValue.asset == null)
					{
						continue;
					}

					draggedNodeReferences[assetValue.asset] = node;
					draggedUnityObjects.Add(assetValue.asset);
					draggedObjects.RemoveAt(index);
					
					DragAndDropContextHandler.SetDirty();
				}
			}
			else
			{
				for (var index = draggedUnityObjects.Count - 1; index >= 0; index--)
				{
					var obj = draggedUnityObjects[index];
					
					if (draggedNodeReferences.Remove(obj, out var node) == false)
					{
						continue;
					}
					
					draggedUnityObjects.RemoveAt(index);
					draggedObjects.Add(node);
					
					DragAndDropContextHandler.SetDirty();
				}
			}
		}
	}
}