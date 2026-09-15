using UnityEngine;

namespace Miscalculation.Motion.Common
{
    public static class MotionMath
    {
        public static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

        public static float Ease(string easing, float value)
        {
            float t = Mathf.Clamp01(value);
            switch (easing)
            {
                case "easeInCubic": return t * t * t;
                case "easeOutCubic": return 1f - Mathf.Pow(1f - t, 3f);
                case "easeInOutCubic": return t < .5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * .5f;
                case "easeInOutSine": return -(Mathf.Cos(Mathf.PI * t) - 1f) * .5f;
                case "easeOutBack":
                    const float c1 = 1.70158f;
                    const float c3 = c1 + 1f;
                    return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
                default: return t;
            }
        }

        public static uint Hash32(string value)
        {
            unchecked
            {
                uint hash = 2166136261u;
                string text = value ?? string.Empty;
                for (int i = 0; i < text.Length; i++)
                {
                    hash ^= text[i];
                    hash *= 16777619u;
                }
                return hash;
            }
        }

        public static float SeededUnit(string seed) => Hash32(seed) / 4294967296f;

        public static Vector3 SeededSigned3(string seed)
        {
            return new Vector3(
                SeededUnit(seed + ":x") * 2f - 1f,
                SeededUnit(seed + ":y") * 2f - 1f,
                SeededUnit(seed + ":z") * 2f - 1f);
        }

        public static Vector2 WorldPivotIn(RectTransform value, RectTransform coordinateRoot)
        {
            if (!value || !coordinateRoot) return Vector2.zero;
            Vector3 local = coordinateRoot.InverseTransformPoint(value.position);
            return new Vector2(local.x, local.y);
        }
    }
}
