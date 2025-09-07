using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LordSheo
{
    public class MonoGizmo : MonoBehaviour
    {
        public Color color = Color.white;

        public void OnDrawGizmos()
        {
            var gizColor = Gizmos.color;
            Gizmos.color = color;

            OnDrawGizmos_Internal();
            
            Gizmos.color = gizColor;
        }

        protected virtual void OnDrawGizmos_Internal()
        {
        }

        public virtual void DestroyAfter(float seconds)
        {
            Destroy(gameObject, seconds);
        }
    }
}
