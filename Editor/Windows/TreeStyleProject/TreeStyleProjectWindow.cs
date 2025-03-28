using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	// NOTE: This MIGHT just serialise correctly?
	public class TreeStyleProjectWindow : OdinMenuEditorWindow
	{
		public NodeGraph<IValue> graph = new();

		public bool IsSearching => string.IsNullOrEmpty(MenuTree.Config.SearchTerm) == false;

		private JsonSerializerSettings _settings;
		private NodeGraphSerialiser<IValue> _serialiser;

		[MenuItem("Window/" + ConstValues.NAMESPACE_PATH + nameof(TreeStyleProjectWindow))]
		private static TreeStyleProjectWindow Open()
		{
			return TreeStyleProjectWindow.CreateWindow<TreeStyleProjectWindow>();
		}

		private void Init()
		{
			// TO-DO: Clean this up
			_settings ??= new JsonSerializerSettings()
			{
				Converters = new List<JsonConverter>()
				{
					new TSPJsonConverter()
				}
			};

			_serialiser ??= new NodeGraphSerialiser<IValue>(_settings);
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			Init();

			var json = ProjectEditorPrefs.Instance.GetString(nameof(TreeStyleProjectWindow), "");
			graph = _serialiser.Deserialise(json) ?? new();

			InternalForceMenuTreeRebuild();
		}

#if ODIN_INSPECTOR_3_3
		protected override void OnImGUI()
		{
			EventHandler.Update(Event.current);

			MenuWidth = position.width;

			base.OnImGUI();
		}
#else
		protected override void OnGUI()
		{
			EventHandler.Update(Event.current);

			MenuWidth = position.width;

			base.OnGUI();	
		}
#endif

		protected override OdinMenuTree BuildMenuTree()
		{
			var tree = new OdinMenuTree();
			tree.Config.DrawSearchToolbar = true;

			tree.Selection.SelectionChanged += OnSelectionChanged;

			foreach (var child in graph.children)
			{
				try
				{
					var parent = Add(tree, child, "");
					AddChildren(tree, child, parent);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}

			return tree;
		}

		private void OnSelectionChanged(SelectionChangedType type)
		{
			if (type == SelectionChangedType.ItemAdded)
			{
				var nodes = MenuTree.Selection.SelectedValues
					.OfType<Node<IValue>>()
					.Select(n => n.value);

				foreach (var value in nodes)
				{
					value.Select();
				}
			}
		}

		private void AddChildren(OdinMenuTree tree, Node<IValue> root, string parent)
		{
			foreach (var node in root.children)
			{
				var name = Add(tree, node, parent);

				AddChildren(tree, node, name);
			}
		}

		private string Add(OdinMenuTree tree, Node<IValue> node, string parent)
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

		private void OnAddNode(Node<IValue> node, OdinMenuItem item)
		{
			item.OnDrawItem += OnDrawMenuItem;
		}

		private void OnDrawMenuItem(OdinMenuItem item)
		{
			var node = item.Value as Node<IValue>;
			node.value.OnGUI(item.Rect);

			if (IsSearching == false)
			{
				DragAndDropUtilities.DragZone(item.Rect, node, true, true);

				HandleAssetDropZone(node, item.Rect);
				HandleNodeDropZone(node, item.Rect);
			}

			HandleMenuItemMiddleClick(item, node);
		}

		private void HandleMenuItemMiddleClick(OdinMenuItem item, Node<IValue> node)
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
			ForceAllMenuTreeRebuild();

			Event.current.Use();
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

			var isDraggingNode = DragAndDropUtilitiesProxy.GetCombinedDraggedObject().Any(o => o is Node<IValue>);

			if (isDraggingNode)
			{
				HandleNodeDropZone(graph, rect);
			}
			else
			{
				HandleAssetDropZone(graph, rect);
			}
		}

		private void HandleNodeDropZone(Node<IValue> parent, Rect rect)
		{
			var droppedNodes = DragAndDropUtilitiesProxy.DropZone<Node<IValue>>(rect);

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

		private void HandleAssetDropZone(Node<IValue> parent, Rect rect)
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

				var node = new Node<IValue>(value);

				parent.AddChild(node);
			}

			ForceAllMenuTreeRebuild();
		}

		public void ForceAllMenuTreeRebuild()
		{
			var windows = WindowUtil.GetOpenWindows<TreeStyleProjectWindow>();

			foreach (var window in windows)
			{
				window.InternalForceMenuTreeRebuild();
			}
		}

		public void InternalForceMenuTreeRebuild()
		{
			var json = _serialiser.Serialise(graph);

			ProjectEditorPrefs.Instance.SetString(nameof(TreeStyleProjectWindow), json);

			Debug.Log(json);

			ForceMenuTreeRebuild();
			Repaint();
		}
	}
}