using UnityEngine;

namespace LordSheo
{
	public static class GizmosUtil
	{
		public static SphereGizmo Sphere(Vector3 position, float radius, Color color)
		{
			var comp = new GameObject("SPHERE GIZMO")
				.AddComponent<SphereGizmo>();

			comp.color = color;
			comp.radius = radius;
            
			comp.transform.position = position;

			return comp;
		}
	}
}