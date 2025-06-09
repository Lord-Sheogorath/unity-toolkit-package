using System;

namespace LordSheo.Editor
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public class TypeGuidAttribute : Attribute
	{
		public readonly string guid;

		public TypeGuidAttribute(string guid)
		{
			this.guid = guid;
		}
	}
}