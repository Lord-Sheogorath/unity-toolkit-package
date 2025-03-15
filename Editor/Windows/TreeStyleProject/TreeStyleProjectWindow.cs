using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Windows
{
	public interface ITreeStyleProjectValue
	{
		string Name { get; }
		Texture Icon { get; }

		bool IsValid();
		void Refresh();
		void Select();
		void OnGUI(Rect rect);
	}

	public class TSP_AssetValue : ITreeStyleProjectValue
	{
		public string guid;
		public string name;

		[JsonIgnore]
		public UnityEngine.Object asset;

		[JsonIgnore]
		public string Name => IsValid() ? name : guid;

		[JsonIgnore]
		public Texture Icon { get; set; }

		[JsonIgnore]
		public OdinMenuItem Item { get; set; }

		public bool IsValid()
		{
			return string.IsNullOrEmpty(guid) == false 
				&& asset != null;
		}
		public void Refresh()
		{
			if (IsValid() == false)
			{
				asset = null;
				Icon = null;
			}

			var path = AssetDatabase.GUIDToAssetPath(guid);

			name = path.Split("/").LastOrDefault();
			asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

			Icon = AssetDatabase.GetCachedIcon(path);
		}
		public void Select()
		{
		}
		public void OnGUI(Rect rect)
		{
			if (EventHandler.IsKeyDown(KeyCode.LeftControl))
			{
				var color = Color.cyan;
				color.a = 0.25f;

				EditorGUI.DrawRect(rect, color);
				DragAndDropUtilities.DragZone<UnityEngine.Object>(rect, asset, true, true);
			}

			if (rect.Contains(Event.current.mousePosition) == false)
			{
				return;
			}

			if (Event.current.clickCount == 1)
			{
				Selection.activeObject = asset;
				EditorGUIUtility.PingObject(asset);
			}
			else if (Event.current.clickCount == 2)
			{
				if (TreeStyleProjectSettings.Instance.enableEditWithDoubleClick)
				{
					AssetDatabase.OpenAsset(asset);
				}
			}
		}
	}

	public class TreeStyleProjectWindow : OdinMenuEditorWindow
	{
		public NodeGraph<ITreeStyleProjectValue> graph = new();

		public bool IsSearching => string.IsNullOrEmpty(MenuTree.Config.SearchTerm) == false;

		[MenuItem("Windows/" + ConstValues.NAMESPACE_PATH + nameof(TreeStyleProjectWindow))]
		private static TreeStyleProjectWindow Open()
		{
			return TreeStyleProjectWindow.CreateWindow<TreeStyleProjectWindow>();
		}

		protected override void OnImGUI()
		{
			EventHandler.Update(Event.current);

			MenuWidth = position.width;

			base.OnImGUI();
		}

		protected override OdinMenuTree BuildMenuTree()
		{
			var tree = new OdinMenuTree();
			tree.Config.DrawSearchToolbar = true;

			tree.Selection.SelectionChanged += OnSelectionChanged;

			foreach (var child in graph.children)
			{
				var parent = Add(tree, child, "");
				AddChildren(tree, child, parent);
			}

			return tree;
		}

		private void OnSelectionChanged(SelectionChangedType type)
		{
			if (type == SelectionChangedType.ItemAdded)
			{
				var nodes = MenuTree.Selection.SelectedValues
					.OfType<Node<ITreeStyleProjectValue>>()
					.Select(n => n.value);

				foreach (var value in nodes)
				{
					value.Select();
				}
			}
		}

		private void AddChildren(OdinMenuTree tree, Node<ITreeStyleProjectValue> root, string parent)
		{
			foreach (var node in root.children)
			{
				var name = Add(tree, node, parent);

				AddChildren(tree, node, name);
			}
		}
		private string Add(OdinMenuTree tree, Node<ITreeStyleProjectValue> node, string parent)
		{
			node.value.Refresh();

			var name = "[Missing] " + node.value.Name;

			if (node.value.IsValid())
			{
				name = node.value.Name;
			}

			var path = name;

			if (string.IsNullOrEmpty(parent) == false)
			{
				path = parent + "/" + name;
			}

			var item = new OdinMenuItem(tree, name, node);

			tree.AddMenuItemAtPath(parent, item);

			item.IconGetter = GetItemIcon;

			OnAddNode(node, item);

			return path;

			Texture GetItemIcon()
			{
				if (node.value.IsValid())
				{
					return node.value.Icon;
				}
				else
				{
					return EditorIcons.AlertCircle.Active;
				}
			}
		}

		private void OnAddNode(Node<ITreeStyleProjectValue> node, OdinMenuItem item)
		{
			item.OnDrawItem += OnDrawMenuItem;
		}

		private void OnDrawMenuItem(OdinMenuItem item)
		{
			var node = item.Value as Node<ITreeStyleProjectValue>;

			node.value.OnGUI(item.Rect);

			if (IsSearching == false)
			{ 
				DragAndDropUtilities.DragZone(item.Rect, node, true, true);

				HandleAssetDropZone(node, item.Rect);
				HandleNodeDropZone(node, item.Rect);
			}

			HandleMenuItemMiddleClick(item, node);

		}

		private void HandleMenuItemMiddleClick(OdinMenuItem item, Node<ITreeStyleProjectValue> node)
		{
			if (Event.current.button != 2 || Event.current.type != EventType.MouseUp)
			{
				return;
			}

			if (item.Rect.Contains(Event.current.mousePosition) == false)
			{
				return;
			}

			if (TreeStyleProjectSettings.Instance.deleteChildrenWithParent == false)
			{
				foreach (var child in node.children.ToArray())
				{
					node.parent?.AddChild(child);
				}
			}

			node.parent?.RemoveChild(node);
			ForceMenuTreeRebuild();

			Event.current.Use();
		}

		protected override void DrawMenu()
		{
			var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

			SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
			GUILayout.FlexibleSpace();

			if (SirenixEditorGUI.ToolbarButton("Refresh"))
			{
				ForceMenuTreeRebuild();
			}
			if (SirenixEditorGUI.ToolbarButton("Clear"))
			{
				var children = graph.children.ToList();

				foreach (var node in children)
				{
					graph.RemoveChild(node);
				}

				ForceMenuTreeRebuild();
			}

			SirenixEditorGUI.EndHorizontalToolbar();

			base.DrawMenu();

			HandleMenuDropZone();
		}

		private void HandleMenuDropZone()
		{
			if (string.IsNullOrEmpty(MenuTree.Config.SearchTerm) == false)
			{
				return;
			}

			// NOTE: Need to calculate remaining space after
			// last item in the tree.
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();

			var rect = GUILayoutUtility.GetLastRect();

			var isDraggingNode = DragAndDropUtilitiesProxy.GetCombinedDraggedObject().Any(o => o is Node<ITreeStyleProjectValue>);

			if (isDraggingNode)
			{
				HandleNodeDropZone(graph, rect);
			}
			else
			{
				HandleAssetDropZone(graph, rect);
			}
		}

		private void HandleNodeDropZone(Node<ITreeStyleProjectValue> parent, Rect rect)
		{
			var droppedNodes = DragAndDropUtilitiesProxy.DropZone<Node<ITreeStyleProjectValue>>(rect);

			if (droppedNodes != null)
			{
				foreach (var droppedNode in droppedNodes)
				{
					parent.AddChild(droppedNode);
				}

				if (droppedNodes.Length > 0)
				{
					ForceMenuTreeRebuild();
				}
			}
		}

		private void HandleAssetDropZone(Node<ITreeStyleProjectValue> parent, Rect rect)
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

				var value = new TSP_AssetValue()
				{
					guid = guid.ToString()
				};

				value.Refresh();

				var node = new Node<ITreeStyleProjectValue>(value);

				parent.AddChild(node);
			}

			ForceMenuTreeRebuild();
		}
	}
}
