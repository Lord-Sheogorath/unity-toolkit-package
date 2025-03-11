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
		OdinMenuItem Item { get; set; }

		bool IsValid();
		void Refresh();
		void Select();
	}

	public class TSP_AssetValue : ITreeStyleProjectValue
	{
		public string guid;
		public string name;

		[JsonIgnore]
		public UnityEngine.Object asset;

		[JsonIgnore]
		public string Name => name;

		[JsonIgnore]
		public Texture Icon { get; set; }

		[JsonIgnore]
		public OdinMenuItem Item { get; set; }

		public bool IsValid()
		{
			return string.IsNullOrEmpty(guid) == false;
		}
		public void Refresh()
		{
			if (IsValid() == false)
			{
				asset = null;
				Icon = null;
			}

			var path = AssetDatabase.GUIDToAssetPath(guid);

			name = path.Split("/")[^1];
			asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);

			Icon = AssetDatabase.GetCachedIcon(path);
		}
		public void Select()
		{
			Selection.activeObject = asset;
			EditorGUIUtility.PingObject(asset);
		}
	}

	public class TreeStyleProjectWindow : OdinMenuEditorWindow
	{
		public NodeGraph<ITreeStyleProjectValue> graph = new();

		[MenuItem("Windows/" + ConstValues.NAMESPACE_PATH + nameof(TreeStyleProjectWindow))]
		private static TreeStyleProjectWindow Open()
		{
			return TreeStyleProjectWindow.CreateWindow<TreeStyleProjectWindow>();
		}

		protected override void OnImGUI()
		{
			MenuWidth = position.width;

			base.OnImGUI();
		}

		protected override OdinMenuTree BuildMenuTree()
		{
			var tree = new OdinMenuTree();

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
			node.value.Item = item;

			tree.AddMenuItemAtPath(parent, item);

			if (node.value.IsValid())
			{
				item.Icon = node.value.Icon;
			}
			else
			{
				item.Icon = EditorIcons.AlertCircle.Active;
			}

			OnAddNode(node, item);

			return path;
		}

		private void OnAddNode(Node<ITreeStyleProjectValue> node, OdinMenuItem item)
		{
			item.OnDrawItem += OnDrawMenuItem;
		}

		private void OnDrawMenuItem(OdinMenuItem item)
		{
			var node = item.Value as Node<ITreeStyleProjectValue>;
			HandleDropZone(node, item.Rect);
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

			// NOTE: Need to calculate remaining space after
			// last item in the tree.
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();

			var rect = GUILayoutUtility.GetLastRect();

			HandleDropZone(graph, rect);
		}

		private void HandleDropZone(Node<ITreeStyleProjectValue> parent, Rect rect)
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
