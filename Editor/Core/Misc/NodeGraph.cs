using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LordSheo.Editor
{
	public class Node<T>
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

		public void AddChild(Node<T> node)
		{
			var isSwappingParents = node == parent;

			// Handle adding a parent to a child.
			if (isSwappingParents)
			{
				node.RemoveChild(this);
				node.parent?.AddChild(this);
			}

			if (node.parent != null)
			{
				node.parent.RemoveChild(node);
			}
			
			node.parent = this;
			children.Add(node);

			addedCallback?.Invoke(node);
			modifiedCallback?.Invoke();
		}
		public void RemoveChild(Node<T> node)
		{
			var removed = children.Remove(node);

			if (removed == false)
			{
				return;
			}

			node.parent = null;

			removedCallback?.Invoke(node);
			modifiedCallback?.Invoke();
		}

		public void Refresh()
		{
			foreach (var child in children)
			{
				child.parent = this;
				child.Refresh();
			}
		}

		public IEnumerable<Node<T>> GetChildNodesRecursive(bool includeSelf)
		{
			if (includeSelf)
			{
				yield return this;
			}

			var nodes = children.SelectMany(n => n.GetChildNodesRecursive(includeSelf: true));

			foreach (var item in nodes)
			{
				yield return item;
			}
		}
	}

	public class NodeGraph<T> : Node<T>
	{
	}

	public class NodeGraphSerialiser<T>
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
