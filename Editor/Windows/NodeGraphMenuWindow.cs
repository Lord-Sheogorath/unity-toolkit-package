using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LordSheo.Editor.Windows
{
	public class NodeGraphMenuWindow<T> : OdinMenuEditorWindow
		where T : INodeValue
	{
		[NonSerialized]
		public bool isDirty;
		
		public bool IsSearching => string.IsNullOrEmpty(MenuTree.Config.SearchTerm) == false;
		
		protected NodeGraph<T> graph;

		protected JsonSerializerSettings settings;
		protected NodeGraphSerialiser<T> serialiser;

		protected virtual bool DeleteChildrenWithParent { get; } = false;
		
		protected virtual void Setup()
		{
			settings ??= new JsonSerializerSettings();
			serialiser ??= new NodeGraphSerialiser<T>(settings);

			graph ??= new();
			graph.Refresh();

			InternalForceMenuTreeRebuild();
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			Setup();
		}
		
#if ODIN_INSPECTOR_3_3
		protected override void OnImGUI()
		{
			InternalOnGUI();
			base.OnImGUI();
		}
#else
		protected override void OnGUI()
		{
			InternalOnGUI();
			base.OnGUI();	
		}
#endif

		protected virtual void InternalOnGUI()
		{
			EventHandler.Update(Event.current);

			MenuWidth = position.width;

			var selectedItems = MenuTree.Selection.ToArray();
			var selectedItemGuids = selectedItems
				.Select(i => i.Value)
				.OfType<Node<T>>()
				.Select(n => n.guid)
				.ToHashSet();
			
			HandleArrowControls(selectedItems);

			if (isDirty)
			{
				InternalForceMenuTreeRebuild();

				var itemsToSelect = MenuTree.EnumerateTree()
					.Select(i => (item: i, node: i.Value as Node<T>))
					.Where(v => v.node != null)
					.Where(v => selectedItemGuids.Contains(v.node.guid))
					.Select(v => v.item)
					.ToArray();
				
				MenuTree.Selection.Clear();

				foreach (var item in itemsToSelect)
				{
					MenuTree.Selection.Add(item);
				}
			}
		}
		
		protected virtual void HandleArrowControls(OdinMenuItem[] selectedItems)
		{
			if (Event.current.alt == false)
			{
				return;
			}

			if (Event.current.type != EventType.KeyDown)
			{
				return;
			}

			HandleChildOrderControls(selectedItems);
			HandleParentControls(selectedItems);
		}

		protected virtual void HandleChildOrderControls(OdinMenuItem[] selectedItems)
		{
			var childIndexOffset = 0;
			var keyCode = Event.current.keyCode;

			if (keyCode == KeyCode.UpArrow)
			{
				childIndexOffset = -1;
			}
			else if (keyCode == KeyCode.DownArrow)
			{
				childIndexOffset = 1;
			}

			if (childIndexOffset == 0)
			{
				return;
			}

			foreach (var item in selectedItems)
			{
				var node = item.Value as Node<T>;

				if (node == null || node.parent == null)
				{
					continue;
				}

				var parent = node.parent;

				var currentChildIndex = parent.children.IndexOf(node);

				var nextChildIndex = currentChildIndex + childIndexOffset;
				nextChildIndex = Mathf.Clamp(nextChildIndex, 0, parent.children.Count - 1);

				var temp = parent.children[nextChildIndex];

				parent.children[currentChildIndex] = temp;
				parent.children[nextChildIndex] = node;

				if (currentChildIndex != nextChildIndex)
				{
					SetWindowDirty();
					Event.current.Use();
				}
			}
		}

		protected virtual void HandleParentControls(OdinMenuItem[] selectedItems)
		{
			var parentIndexOffset = 0;
			var keyCode = Event.current.keyCode;
			
			if (keyCode == KeyCode.LeftArrow)
			{
				parentIndexOffset = -1;
			}
			else if (keyCode == KeyCode.RightArrow)
			{
				parentIndexOffset = 1;
			}

			if (parentIndexOffset == 0)
			{
				return;
			}

			foreach (var item in selectedItems)
			{
				var node = item.Value as Node<T>;

				if (node == null)
				{
					continue;
				}
				
				var parent = node.parent;
				
				if (parentIndexOffset == -1)
				{
					if (parent == graph)
					{
						continue;
					}

					var grandParent = parent.parent;

					if (grandParent == null)
					{
						continue;
					}

					var parentChildIndex = grandParent.children.IndexOf(parent);
					
					parent.RemoveChild(node);
					grandParent.AddChild(node);

					grandParent.children.Remove(node);
					grandParent.children.Insert(parentChildIndex + 1, node);
					
					SetWindowDirty();
				}
				else if (parentIndexOffset == 1)
				{
					var currentChildIndex = parent.children.IndexOf(node);

					if (currentChildIndex == 0)
					{
						continue;
					}

					var prevChildIndex = currentChildIndex - 1;
					var prevChild = parent.children[prevChildIndex];

					parent.RemoveChild(node);
					prevChild.AddChild(node);
					
					SetWindowDirty();
				}
			}
		}
		
		protected virtual void DrawNodeDraggingHandle()
		{
			var draggedNode = DragAndDropUtilitiesProxy.GetDraggedObjects()
				.OfType<Node<T>>()
				.FirstOrDefault();

			if (draggedNode == null)
			{
				return;
			}

			var draggedItem = MenuTree
				.EnumerateTree()
				.FirstOrDefault(i =>
				{
					var node = i.Value as Node<T>;

					if (node == null)
					{
						return false;
					}

					return node == draggedNode;
				});

			if (draggedItem == null)
			{
				return;
			}

			var rect = draggedItem.Rect;
			rect.position = Event.current.mousePosition;

			// TO-DO: Make this work outside of the TSP window.
			EditorGUI.DrawRect(rect, Color.blue);
		}
		
		protected override OdinMenuTree BuildMenuTree()
		{
			var tree = new OdinMenuTree();

			try
			{
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
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}

			return tree;
		}

		protected virtual void OnSelectionChanged(SelectionChangedType type)
		{
			if (type == SelectionChangedType.ItemAdded)
			{
				var nodes = MenuTree.Selection.SelectedValues
					.OfType<Node<T>>()
					.Select(n => n.value);

				foreach (var value in nodes)
				{
					value.Select();
				}
			}
		}

		protected virtual void AddChildren(OdinMenuTree tree, Node<T> root, string parent)
		{
			foreach (var node in root.children)
			{
				var name = Add(tree, node, parent);

				AddChildren(tree, node, name);
			}
		}

		protected virtual string Add(OdinMenuTree tree, Node<T> node, string parent)
		{
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

		protected virtual void OnAddNode(Node<T> node, OdinMenuItem item)
		{
			item.OnDrawItem += OnDrawMenuItem;

			node.modifiedCallback -= SetWindowDirty;
			node.modifiedCallback += SetWindowDirty;
		}

		protected virtual void OnDrawMenuItem(OdinMenuItem item)
		{
			var node = item.Value as Node<T>;
			node.value.OnGUI(item.Rect);

			HandleMenuItemMiddleClick(item, node);
		}
		
		protected virtual void HandleMenuItemMiddleClick(OdinMenuItem item, Node<T> node)
		{
			if (Event.current.IsMouseUp(2) == false)
			{
				return;
			}

			if (item.Rect.Contains(Event.current.mousePosition) == false)
			{
				return;
			}

			if (DeleteChildrenWithParent)
			{
				foreach (var child in node.children.ToArray())
				{
					node.parent?.AddChild(child);
				}
			}

			node.parent?.RemoveChild(node);
			InternalForceMenuTreeRebuild();

			Event.current.Use();
		}
		
		public virtual void InternalForceMenuTreeRebuild()
		{
			isDirty = false;

			var focused = EditorWindow.focusedWindow;

			ForceMenuTreeRebuild();
			Repaint();

			if (focused == this)
			{
				OdinMenuTree.ActiveMenuTree = MenuTree;
				EditorApplicationUtility.ForceFocusWindow(this);
			}
		}
		public virtual void SetWindowDirty()
		{
			isDirty = true;
		}
	}
}