using UnityEngine;

namespace LordSheo
{
	public class SphereGizmo : MonoGizmo
	{
		public float radius;
		public bool wire = true;
        
		protected override void OnDrawGizmos_Internal()
		{
			if (wire)
			{
				Gizmos.DrawWireSphere(transform.position, radius);
			}
			else
			{
				Gizmos.DrawSphere(transform.position, radius);
			}
		}
	}
}