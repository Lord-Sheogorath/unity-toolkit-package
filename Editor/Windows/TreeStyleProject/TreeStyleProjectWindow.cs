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

			Reload();

			base.Setup();
		}

		protected void Reload()
		{
			var json = EditorProject.projectOnlyPrefs.GetString(nameof(TreeStyleProjectWindow), "");
			Load(json);
		}

		protected void Load(string json)
		{
			graph = serialiser.Deserialise(json) ?? new();
			graph.Refresh();
			
			InternalForceMenuTreeRebuild();
		}

		protected void Save()
		{
			try
			{
				var json = serialiser.Serialise(graph);

				EditorProject.projectOnlyPrefs.SetString(nameof(TreeStyleProjectWindow), json);

				Debug.Log(json);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
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

			if (SirenixEditorGUI.ToolbarButton("Import"))
			{
				EditorApplicationUtility.DisplayConfirmAction("TSP - Import", "Are you sure?", () =>
				{
					Load(GUIUtility.systemCopyBuffer);
					Save();	
				});
			}

			if (SirenixEditorGUI.ToolbarButton("Export"))
			{
				GUIUtility.systemCopyBuffer = serialiser.Serialise(graph);
			}

			GUILayout.FlexibleSpace();

			if (SirenixEditorGUI.ToolbarButton("Refresh"))
			{
				Reload();
				ForceAllMenuTreeRebuild();
			}

			if (SirenixEditorGUI.ToolbarButton("Clear"))
			{
				EditorApplicationUtility.DisplayConfirmAction("TSP - Clear", "Are you sure?", () =>
				{
					var children = graph.children.ToList();

					foreach (var node in children)
					{
						graph.RemoveChild(node);
					}

					ForceAllMenuTreeRebuild();
				});
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

			AddAssets(parent, dropped);
		}

		private void AddAssets(Node<ITreeStyleValue> parent, UnityEngine.Object[] assets)
		{
			for (int i = 0; i < assets.Length; i++)
			{
				var asset = assets[i];

				var path = AssetDatabase.GetAssetPath(asset);
				var guid = AssetDatabase.GUIDFromAssetPath(path);

				Debug.Log(path);

				var value = new AssetValue()
				{
					guid = guid.ToString()
				};

				value.Refresh();
				var node = new Node<ITreeStyleValue>(value);

				// Handle trying to add the same asset at the same level
				// multiple times and still prompt user to add folder contents.
				var duplicateChildNode = parent.children.FirstOrDefault(c => c.value.Name == value.Name);
				
				if (duplicateChildNode != null)
				{
					node = duplicateChildNode;
				}
				else
				{
					parent.AddChild(node);
				}

				AddFolderAssetContents(node);
			}

			ForceAllMenuTreeRebuild();
		}

		private void AddFolderAssetContents(Node<ITreeStyleValue> node, bool rebuild = false)
		{
			var asset = node.value as AssetValue;
			
			if (AssetDatabase.IsValidFolder(asset.path) == false)
			{
				return;
			}

			var guids = AssetDatabase.FindAssets(null, new string[] { asset.path });

			if (guids.Length == 0)
			{
				return;
			}

			var confirm = EditorUtility.DisplayDialog("TSP - Add Folder Contents", $"Folder '{asset.path}' has {guids.Length} assets", "Add", "Skip");

			if (confirm == false)
			{
				return;
			}
			
			var pathToGuid = guids.ToDictionary(g => AssetDatabase.GUIDToAssetPath(g), g => g);

			var paths = pathToGuid.Keys
				.OrderBy(p => p.Length)
				.ToArray();

			var nodes = new Dictionary<string, Node<ITreeStyleValue>>();
			nodes[asset.path] = node;

			foreach (var path in paths)
			{
				var split = path.Split("/");

				var assetParentPath = string.Join("/", split.Take(split.Length - 1));
				var parent = nodes[assetParentPath];

				var guid = pathToGuid[path];
				
				var assetValue = new AssetValue()
				{
					guid = guid,
				};
				
				assetValue.Refresh();
				
				var child = new Node<ITreeStyleValue>(assetValue);

				parent.AddChild(child);

				nodes[path] = child;
			}

			if (rebuild)
			{
				ForceAllMenuTreeRebuild();
			}
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
			Save();
			base.InternalForceMenuTreeRebuild();
		}
	}
}