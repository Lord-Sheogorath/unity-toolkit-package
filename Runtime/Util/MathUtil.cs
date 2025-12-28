using UnityEngine;

namespace LordSheo
{
    public static class MathUtil
    {
        public static float Remap(float value, float srcMin, float srcMax, float toMin,  float toMax)
        {
            var fromAbs  =  value - srcMin;
            var fromMaxAbs = srcMax - srcMin;       
       
            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;
       
            return to;
        }

        public static float Remap01(float value, float toMin, float toMax)
        {
            return Remap(value, 0, 1, toMin, toMax);
        }
        
        public static Vector3 Remap01(Vector3 value, float toMin, float toMax)
        {
            if (value == Vector3.zero)
            {
                return Vector3.zero;
            }

            var mag = value.magnitude;
            
            return value.normalized *
                Remap(mag, 0f, 1f, toMin, toMax);
        }
    }
}
