using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace LordSheo.Editor
{
	public static class TypeGuidUtil
	{
		public static readonly Dictionary<Type, TypeGuidAttribute> types = new();
		
		static TypeGuidUtil()
		{
			var typeCollection = TypeCache.GetTypesWithAttribute<TypeGuidAttribute>();

			foreach (var type in typeCollection)
			{
				types[type] = (TypeGuidAttribute)Attribute.GetCustomAttribute(type, typeof(TypeGuidAttribute));
			}
		}

		public static TypeGuidAttribute GetGuid(Type type)
		{
			return types.GetValueOrDefault(type);
		}

		public static Type GetType(string guid)
		{
			return types.FirstOrDefault(kvp => kvp.Value.guid == guid).Key;
		}
	}
}