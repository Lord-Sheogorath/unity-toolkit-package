using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	public interface ITreeStyleValue : INodeValue
	{
		IEnumerable<GenericSelectorItem<System.Action>> GetContextActions();
	}
}