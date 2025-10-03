#if LORD_SHEO_ODIN_ENABLED
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace LordSheo.Editor.Windows.TSP
{
	public interface ITreeStyleValue : INodeValue
	{
		event System.Action ModifiedCallback;
		
		IEnumerable<GenericSelectorItem<System.Action>> GetContextActions();
	}
}
#endif