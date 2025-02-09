namespace Utils
{
    using UnityEngine;

    public static class DebugUtils
    {
        public static void DrawCircle(Vector3 center, float radius, Color color, float duration = 0f)
        {
            int segments = 36;
            float angle = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float currentAngle = angle * i;
                float nextAngle = angle * (i + 1);

                Vector3 currentPoint = center + Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius;
                Vector3 nextPoint = center + Quaternion.Euler(0, 0, nextAngle) * Vector3.right * radius;

                Debug.DrawLine(currentPoint, nextPoint, color, duration);
            }
        }

        public static void LogWithColor(string message, string hexColor)
        {
            Debug.Log($"<color={hexColor}>{message}</color>");
        }
    }
}