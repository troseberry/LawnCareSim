using UnityEngine;

namespace Core.Utility
{
    public static class MathHelpers
    {
        public static Vector3 Clamp(this Vector3 vector, Vector3 min, Vector3 max)
        {
            var x = Mathf.Clamp(vector.x, min.x, max.x);
            var y = Mathf.Clamp(vector.y, min.y, max.y);
            var z = Mathf.Clamp(vector.z, min.z, max.z);

            return new Vector3(x, y, z);
        }

        public static Vector3 Clamp(this Vector3 vector, Vector2 xRange, Vector2 zRange)
        {
            var x = Mathf.Clamp(vector.x, xRange.x, xRange.y);
            var z = Mathf.Clamp(vector.z, zRange.x, zRange.y);

            return new Vector3(x, vector.y, z);
        }

        public static bool IsWithinRange(float value, float compValue, float margin)
        {
            return (value <= compValue + margin) && (value >= compValue - margin);
        }
    }
}
