using UnityEngine;

namespace LordSheo
{
	public static class VectorExtensions
	{
		public static Vector2 X(this Vector2 source, float x)
		{
			source.x = x;
			return source;
		}
		public static Vector2 Y(this Vector2 source, float y)
		{
			source.y = y;
			return source;
		}
		
		public static bool In(this Vector2 source, Vector2 other, float dist)
		{
			dist *= dist;
			return Vector2.SqrMagnitude(source - other) <= dist;
		}
		public static bool Out(this Vector2 source, Vector2 other, float dist)
		{
			return source.In(other, dist) == false;
		}
		
		public static Vector3 X(this Vector3 source, float x)
		{
			source.x = x;
			return source;
		}
		public static Vector3 Y(this Vector3 source, float y)
		{
			source.y = y;
			return source;
		}
		public static Vector3 Z(this Vector3 source, float z)
		{
			source.z = z;
			return source;
		}
		
		public static Vector3 XY(this Vector3 source, float x, float y)
		{
			source.x = x;
			source.y = y;
			return source;
		}
		public static Vector3 XZ(this Vector3 source, float x, float z)
		{
			source.x = x;
			source.z = z;
			return source;
		}
		public static Vector3 YZ(this Vector3 source, float y, float z)
		{
			source.y = y;
			source.z = z;
			return source;
		}

		public static bool In(this Vector3 source, Vector3 other, float dist)
		{
			dist *= dist;
			return Vector3.SqrMagnitude(source - other) <= dist;
		}
		public static bool Out(this Vector3 source, Vector3 other, float dist)
		{
			return source.In(other, dist) == false;
		}
	}
}