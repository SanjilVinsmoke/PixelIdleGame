
using UnityEngine;

namespace Utils
{
    public class VectorUtils
    {
        public static float Distance(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        public static float CalculateAngleBetweenVectors(Vector3 a, Vector3 b)
        {
            return Vector3.Angle(a, b);
        }

        public static Vector3 NormalizeVector(Vector3 v)
        {
            return v.normalized;
        }

        public static Vector3 AddVectors(Vector3 a, Vector3 b)
        {
            return a + b;
        }

        public static Vector3 MultiplyVector(Vector3 v, float scalar)
        {
            return v * scalar;
        }

        public static float CalculateManhattanDistance(Vector3 a, Vector3 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }

        public static Vector3 RotateVector(Vector3 v, float angleDegrees, Vector3 axis)
        {
            return Quaternion.AngleAxis(angleDegrees, axis) * v;
        }

        public static string FormatVector(Vector3 v)
        {
            return $"({v.x:F2}, {v.y:F2}, {v.z:F2})";
        }

        public static string FormatRect(Rect rect)
        {
            return $"(x:{rect.x:F2}, y:{rect.y:F2}, w:{rect.width:F2}, h:{rect.height:F2})";
        }

        public static Vector3 GetScreenCenter(Camera cam)
        {
            float x = Screen.width / 2;
            float y = Screen.height / 2;
            return cam.ScreenToWorldPoint(new Vector3(x, y, cam.nearClipPlane));
        }
        
        
        
    }
}

