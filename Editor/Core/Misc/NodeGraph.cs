﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace LordSheo.Editor
{
	public interface INodeValue
	{
		public string Name { get; }
		public Texture Icon { get; }
		
		void OnGUI(Rect rect);
		
		bool IsValid();
		void Select();
		void Deselect();
		void Refresh();
	}

	[TypeGuid("BD9A5999-0852-4D53-B585-D4B422A5CD1F")]
	public class Node<T>
		where T : INodeValue
	{
		public string guid;

		public T value;

		[JsonIgnore]
		public Node<T> parent;
		public List<Node<T>> children = new();

		[JsonIgnore]
		public System.Action<Node<T>> addedCallback;
		[JsonIgnore]
		public System.Action<Node<T>> removedCallback;

		[JsonIgnore]
		public System.Action modifiedCallback;

		public Node()
		{
			guid = System.Guid.NewGuid().ToString("N").Substring(0, 8);
		}
		public Node(T value)
			: this()
		{
			this.value = value;
		}

		public virtual void AddChild(Node<T> node, bool silent = false)
		{
			var isSwappingParents = node == parent;

			// Handle adding a parent to a child.
			if (isSwappingParents)
			{
				node.RemoveChild(this, silent);
				node.parent?.AddChild(this, silent);
			}

			if (node.parent != null)
			{
				node.parent.RemoveChild(node, silent);
			}
			
			node.parent = this;
			children.Add(node);

			if (silent)
			{
				return;
			}
			
			addedCallback?.Invoke(node);
			modifiedCallback?.Invoke();
		}
		public virtual void RemoveChild(Node<T> node, bool silent = false)
		{
			var removed = children.Remove(node);

			if (removed == false)
			{
				return;
			}

			node.parent = null;

			if (silent)
			{
				return;
			}

			removedCallback?.Invoke(node);
			modifiedCallback?.Invoke();
		}

		public virtual void MoveChildren(Node<T> parent, bool silent = false)
		{
			while (children.Count > 0)
			{
				var childNode = children[0];
				parent.AddChild(childNode, silent);
			}
		}
		
		public virtual void Refresh()
		{
			if (value != null)
			{
				value.Refresh();
			}

			foreach (var child in children)
			{
				child.parent = this;
				child.Refresh();
			}
		}

		public virtual bool IsValid()
		{
			return value.IsValid();
		}

		public virtual void PerformOnChildren(System.Action<Node<T>> action)
		{
			if (action == null)
			{
				return;
			}
			
			foreach (var child in children)
			{
				action.Invoke(child);
				child.PerformOnChildren(action);
			}
		}

		public virtual IEnumerable<Node<T>> GetAllChildren()
		{
			foreach (var child in children)
			{
				yield return child;

				foreach (var subChild in child.GetAllChildren())
				{
					yield return subChild;
				}
			}
		}

		public virtual int GetAllChildCount()
		{
			var count = 0;
			
			foreach (var child in GetAllChildren())
			{
				count++;
			}

			return count;
		}
	}

	[TypeGuid("7DC896BE-ED12-487F-B1A3-EA5DFD94752F")]
	public class NodeGraph<T> : Node<T>
		where T : INodeValue
	{
	}

	public class NodeGraphSerialiser<T>
		where T : INodeValue
	{
		public readonly JsonSerializerSettings settings;
		
		public NodeGraphSerialiser(JsonSerializerSettings settings)
		{
			this.settings = settings;
		}
		
		public virtual string Serialise(NodeGraph<T> graph)
		{
			return JsonConvert.SerializeObject(graph, settings);
		}
		public virtual NodeGraph<T> Deserialise(string json)
		{
			return JsonConvert.DeserializeObject<NodeGraph<T>>(json, settings);
		}
	}
}
