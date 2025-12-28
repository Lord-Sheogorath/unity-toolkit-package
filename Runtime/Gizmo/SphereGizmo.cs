using System;
using UnityEngine;

namespace LordSheo
{
	public class SphereGizmo : MonoGizmo
	{
		public Transform target;
		public float radius;
		public bool wire = true;

		public void OnValidate()
		{
			if (target == null)
			{
				target = transform;
			}
		}

		protected override void OnDrawGizmos_Internal()
		{
			if (wire)
			{
				Gizmos.DrawWireSphere(target.position, radius);
			}
			else
			{
				Gizmos.DrawSphere(target.position, radius);
			}
		}
	}
}