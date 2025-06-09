using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;

namespace LordSheo.Editor.Windows.TSP
{
	public interface ITreeStyleValue : INodeValue
	{
		IEnumerable<GenericSelectorItem<System.Action>> GetContextActions();
	}
}