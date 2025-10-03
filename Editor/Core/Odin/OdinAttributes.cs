#if !LORD_SHEO_ODIN_ENABLED
using System;

namespace LordSheo.Editor
{
	public class FoldoutGroup : Attribute
	{
		public FoldoutGroup(string group)
		{
		}
	}
	public class PropertyOrderAttribute : Attribute
	{
		public PropertyOrderAttribute(int order)
		{
		}
	}

	public class ShowInInspector : Attribute
	{
	}
	public class HideInInspector : Attribute
	{
	}
}
#endif