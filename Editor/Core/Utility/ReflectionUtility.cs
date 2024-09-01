using System;
using System.Reflection;

namespace Editor
{
	public static class ReflectionUtility
	{
		public static FieldInfo FindFieldInfo(Type type, Type target, string name, BindingFlags flags)
		{
			var field = target.GetField(name, flags);

			if (field != null)
			{
				return field;
			}

			if (target.BaseType != null)
			{
				return FindFieldInfo(type, target.BaseType, name, flags);
			}

			throw new System.ArgumentException($"Could not find field '{name}' on type or base type of: {type.FullName}");

		}
		public static PropertyInfo FindPropertyInfo(Type type, Type target, string name, BindingFlags flags)
		{
			var property = target.GetProperty(name, flags);

			if (property != null)
			{
				return property;
			}

			if (target.BaseType != null)
			{
				return FindPropertyInfo(type, target.BaseType, name, flags);
			}

			throw new System.ArgumentException($"Could not find property '{name}' on type or base type of: {type.FullName}");
		}
		public static MethodInfo FindMethodInfo(Type type, Type target, string name, Type[] args, BindingFlags flags)
		{
			var method = target.GetMethod(name, flags, null, args, null);

			if (method != null)
			{
				return method;
			}

			if (target.BaseType != null)
			{
				return FindMethodInfo(type, target.BaseType, name, args, flags);
			}

			throw new System.ArgumentException($"Could not find method '{name}' on type or base type of: {type.FullName}");
		}
		public static MethodInfo FindMethodInfo(Type type, Type target, string name, BindingFlags flags)
		{
			var method = target.GetMethod(name, flags);

			if (method != null)
			{
				return method;
			}

			if (target.BaseType != null)
			{
				return FindMethodInfo(type, target.BaseType, name, flags);
			}

			throw new System.ArgumentException($"Could not find method '{name}' on type or base type of: {type.FullName}");
		}
	}
}
