using System;
using System.Reflection;
using UnityEngine;

namespace LordSheo.Editor
{
	public static class ReflectionExtensions
	{
		public const BindingFlags STATIC_FLAGS = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
		public const BindingFlags INSTANCE_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		#region Field
		public static FieldInfo FindFieldInfo(this Type type, string name, BindingFlags flags)
		{
			return ReflectionUtility.FindFieldInfo(type, type, name, flags);
		}

		public static FieldInfo FindFieldInfo(this object obj, string name)
		{
			var type = obj.GetType();

			return ReflectionUtility.FindFieldInfo(type, type, name, INSTANCE_FLAGS);
		}

		public static FieldInfo FindFieldInfo_Static(this Type type, string name)
		{
			return ReflectionUtility.FindFieldInfo(type, type, name, STATIC_FLAGS);
		}
		public static FieldInfo FindFieldInfo_Instance(this Type type, string name)
		{
			return ReflectionUtility.FindFieldInfo(type, type, name, INSTANCE_FLAGS);
		}
		#endregion

		#region Property
		public static PropertyInfo FindPropertyInfo(this Type type, string name, BindingFlags flags)
		{
			return ReflectionUtility.FindPropertyInfo(type, type, name, flags);
		}

		public static PropertyInfo FindPropertyInfo(this object obj, string name)
		{
			var type = obj.GetType();

			return ReflectionUtility.FindPropertyInfo(type, type, name, INSTANCE_FLAGS);
		}

		public static PropertyInfo FindPropertyInfo_Static(this Type type, string name)
		{
			return ReflectionUtility.FindPropertyInfo(type, type, name, STATIC_FLAGS);
		}
		public static PropertyInfo FindPropertyInfo_Instance(this Type type, string name)
		{
			return ReflectionUtility.FindPropertyInfo(type, type, name, INSTANCE_FLAGS);
		}
		#endregion

		#region Method
		public static MethodInfo FindMethodInfo(this Type type, string name, Type[] args, BindingFlags flags)
		{
			return ReflectionUtility.FindMethodInfo(type, type, name, args, flags);
		}

		public static MethodInfo FindMethodInfo(this object obj, string name, params Type[] args)
		{
			var type = obj.GetType();

			return ReflectionUtility.FindMethodInfo(type, type, name, args, INSTANCE_FLAGS);
		}

		public static MethodInfo FindMethodInfo_Static(this Type type, string name, params Type[] args)
		{
			return ReflectionUtility.FindMethodInfo(type, type, name, args, STATIC_FLAGS);
		}
		public static MethodInfo FindMethodInfo_Static(this Type type, string name)
		{
			return ReflectionUtility.FindMethodInfo(type, type, name, STATIC_FLAGS);
		}

		public static MethodInfo FindMethodInfo_Instance(this Type type, string name, params Type[] args)
		{
			return ReflectionUtility.FindMethodInfo(type, type, name, args, INSTANCE_FLAGS);
		}
		public static MethodInfo FindMethodInfo_Instance(this Type type, string name)
		{
			return ReflectionUtility.FindMethodInfo(type, type, name, INSTANCE_FLAGS);
		}

		#endregion
	}
}
