using UnityEngine;

namespace Miscalculation.HallMotion
{
    /// <summary>
    /// 主界面动效的运行时数值闸门。
    ///
    /// Unity 的 UI 顶点一旦包含 NaN/Infinity，就可能向上传导为 Invalid AABB，严重时会让
    /// CanvasRenderer 持续报错。所有程序化 Graphic 在提交顶点前都应使用这里的检查，
    /// 发现异常时宁可跳过当前效果，也不能把非法数据交给 Canvas。
    /// </summary>
    internal static class HallMotionRuntimeGuards
    {
        private const float MinimumUsableScale = 0.00001f;

        public static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        public static bool IsFinite(Vector2 value)
        {
            return IsFinite(value.x) && IsFinite(value.y);
        }

        public static bool IsFinite(Vector3 value)
        {
            return IsFinite(value.x) && IsFinite(value.y) && IsFinite(value.z);
        }

        public static bool IsFinite(Vector4 value)
        {
            return IsFinite(value.x) && IsFinite(value.y) && IsFinite(value.z) && IsFinite(value.w);
        }

        public static bool IsFinite(Bounds value)
        {
            return IsFinite(value.center) && IsFinite(value.size);
        }

        public static bool HasUsableScale(Transform target)
        {
            if (target == null)
            {
                return false;
            }

            Vector3 scale = target.lossyScale;
            return IsFinite(scale)
                && Mathf.Abs(scale.x) > MinimumUsableScale
                && Mathf.Abs(scale.y) > MinimumUsableScale
                && Mathf.Abs(scale.z) > MinimumUsableScale;
        }

        public static float NonNegativeSin(float radians)
        {
            // Mathf.Sin(Mathf.PI) 在部分平台/编译设置下会得到极小负数。
            // 先截断为非负值，才能安全用于 0.45 等小数次幂。
            return Mathf.Max(0f, Mathf.Sin(radians));
        }
    }
}
