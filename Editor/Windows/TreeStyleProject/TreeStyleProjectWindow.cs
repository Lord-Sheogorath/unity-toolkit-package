using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	// NOTE: This MIGHT just serialise correctly?
	public class TreeStyleProjectWindow : NodeGraphMenuWindow<ITreeStyleValue>
	{
		protected override bool DeleteChildrenWithParent => TreeStyleProjectSettings.Instance.deleteChildrenWithParent;

		private static OdinEditorWindow activeContextWindow;
		
		[MenuItem("Window/" + ConstValues.NAMESPACE_PATH + nameof(TreeStyleProjectWindow))]
		private static TreeStyleProjectWindow Open()
		{
			var openWindow = WindowUtil.GetOpenWindows<TreeStyleProjectWindow>()
				.FirstOrDefault();

			if (openWindow == null)
			{
				return TreeStyleProjectWindow.CreateWindow<TreeStyleProjectWindow>();
			}

			EditorApplicationUtility.ForceFocusWindow(openWindow);

			return openWindow;
		}

		protected override void Setup()
		{
			settings = new()
			{
				Converters = new List<JsonConverter>()
				{
					new TSPJsonConverter()
				}
			};
			serialiser = new(settings);
			
			var json = ProjectEditorPrefs.Instance.GetString(nameof(TreeStyleProjectWindow), "");
			
			graph = serialiser.Deserialise(json) ?? new();
			
			base.Setup();
		}

		protected override void OnDrawMenuItem(OdinMenuItem item)
		{
			base.OnDrawMenuItem(item);
			
			var node = item.Value as Node<ITreeStyleValue>;

			if (IsSearching == false)
			{
				DragAndDropUtilities.DragZone(item.Rect, node, true, true);

				HandleAssetDropZone(node, item.Rect);
				HandleNodeDropZone(node, item.Rect);
			}
			
			if (item.Rect.Contains(Event.current.mousePosition) == false)
			{
				return;
			}

			if (Event.current.button != 1 || Event.current.type != EventType.MouseUp)
			{
				return;
			}
			
			// Don't want double ups.
			activeContextWindow?.Close();
			
			var actions = node.value
				.GetContextActions()
				.ToArray();

			if (actions.Length > 0)
			{
				var menu = new GenericSelector<System.Action>(item.SmartName, false, actions);
				menu.EnableSingleClickToSelect();
				menu.SelectionTree.Config.DrawSearchToolbar = actions.Length >= 5;
				activeContextWindow = menu.ShowInPopup();

				menu.SelectionConfirmed += selectedActions =>
				{
					foreach (var action in selectedActions)
					{
						try
						{
							action?.Invoke();
						}
						catch (Exception e)
						{
							Debug.LogException(e);
						}
					}
				};

				Event.current.Use();
			}
		}

		protected override void DrawMenu()
		{
			var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

			SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);

			GUILayout.FlexibleSpace();

			if (SirenixEditorGUI.ToolbarButton("Refresh"))
			{
				ForceAllMenuTreeRebuild();
			}

			if (SirenixEditorGUI.ToolbarButton("Clear"))
			{
				var children = graph.children.ToList();

				foreach (var node in children)
				{
					graph.RemoveChild(node);
				}

				ForceAllMenuTreeRebuild();
			}

			SirenixEditorGUI.EndHorizontalToolbar();

			base.DrawMenu();

			// NOTE: Need to calculate remaining space after
			// last item in the tree.
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();

			var rect = GUILayoutUtility.GetLastRect();

			HandleMenuDropZone(rect);
		}

		private void HandleMenuDropZone(Rect rect)
		{
			if (string.IsNullOrEmpty(MenuTree.Config.SearchTerm) == false)
			{
				return;
			}

			var isDraggingNode = DragAndDropUtilitiesProxy.GetCombinedDraggedObject()
				.Any(o => o is Node<ITreeStyleValue>);

			if (isDraggingNode)
			{
				HandleNodeDropZone(graph, rect);
			}
			else
			{
				HandleAssetDropZone(graph, rect);
			}
		}

		private void HandleNodeDropZone(Node<ITreeStyleValue> parent, Rect rect)
		{
			var droppedNodes = DragAndDropUtilitiesProxy.DropZone<Node<ITreeStyleValue>>(rect);

			if (droppedNodes != null)
			{
				foreach (var droppedNode in droppedNodes)
				{
					parent.AddChild(droppedNode);
				}

				if (droppedNodes.Length > 0)
				{
					ForceAllMenuTreeRebuild();
				}
			}
		}

		private void HandleAssetDropZone(Node<ITreeStyleValue> parent, Rect rect)
		{
			var dropped = DragAndDropUtilitiesProxy.DropZone<UnityEngine.Object>(rect);

			if (dropped == null)
			{
				return;
			}

			for (int i = 0; i < dropped.Length; i++)
			{
				var asset = dropped[i];

				var path = AssetDatabase.GetAssetPath(asset);
				var guid = AssetDatabase.GUIDFromAssetPath(path);

				Debug.Log(path);

				var value = new AssetValue()
				{
					guid = guid.ToString()
				};

				value.Refresh();

				var node = new Node<ITreeStyleValue>(value);

				parent.AddChild(node);
			}

			ForceAllMenuTreeRebuild();
		}

		public void ForceAllMenuTreeRebuild()
		{
			var windows = WindowUtil.GetOpenWindows<TreeStyleProjectWindow>();

			foreach (var window in windows)
			{
				window.SetWindowDirty();
			}
		}

		public override void InternalForceMenuTreeRebuild()
		{
			try
			{
				var json = serialiser.Serialise(graph);

				ProjectEditorPrefs.Instance.SetString(nameof(TreeStyleProjectWindow), json);

				Debug.Log(json);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			
			base.InternalForceMenuTreeRebuild();
		}
	}
}