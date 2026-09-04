using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Miscalculation.HallMotion
{
    /// <summary>Parallel 为多线并发曲线；SingleStroke 为不抬笔、左右折返的纯直线折线。</summary>
    public enum MenuScribbleDrawMode
    {
        Parallel = 0,
        SingleStroke = 1
    }

    [CreateAssetMenu(fileName = "MenuScribbleSettings", menuName = "Miscalculation/Menu Scribble Settings")]
    public sealed class MenuScribbleSettings : ScriptableObject
    {
        public const string AlgorithmVersion = "xorshift32-neon-scribble-v11-balanced-soft-lift";
        [Header("Web lab parity")]
        [Tooltip("绘制方式。两种模式共享区域、数量、速度、粗细和颜色，但各自只读取对应模式的专属参数。")]
        public MenuScribbleDrawMode drawMode = MenuScribbleDrawMode.Parallel;
        [FormerlySerializedAs("regionWidth")]
        [Tooltip("霓虹效果区域的最小宽度。实际宽度还会根据悬停目标文字宽度和左右延伸自动增长。")]
        [Range(120f, 520f)] public float minRegionWidth = 260f;
        [Tooltip("相对悬停目标文字左边缘额外伸出的固定像素；更换语言后仍保持相同延伸。")]
        [Range(20f, 220f)] public float leftOverflow = 90f;
        [Tooltip("相对悬停目标文字右边缘额外伸出的固定像素；与左侧相等时视觉严格同心。")]
        [Range(20f, 220f)] public float rightOverflow = 90f;
        [Tooltip("悬停目标文字上下各自增加的特效留白。最终高度取 regionHeight 与文字高度+留白的较大值。")]
        [Range(0f, 70f)] public float verticalPadding = 18f;
        [Tooltip("霓虹效果区域的最小高度；不是强制让所有随机线覆盖到上下边缘。")]
        [Range(44f, 180f)] public float regionHeight = 96f;
        [Tooltip("Parallel 中是并发线条数；SingleStroke 中是左右往返次数。硬上限 16。")]
        [Range(2, 16)] public int lineCount = 7;
        [Tooltip("整组霓虹从起笔到完成的总毫秒数。各线的轻微错时已经包含在算法内。")]
        [Range(80f, 900f)] public float drawDurationMs = 260f;
        [Tooltip("主线中段的基础线芯粗细。首尾柔性提笔会从该基础值按比例计算。")]
        [Range(1f, 9f)] public float thickness = 4f;
        [Tooltip("收笔影响长度。数值越大，首尾淡出区越长；主线中段仍保持均匀，不会产生锯齿。")]
        [Range(0f, 1f)] public float pressureVariation = 0.62f;
        [Tooltip("真实端点保留的最小几何宽度比例。透明度仍降为 0，用来避免实心规则锥尖。")]
        [Range(0.15f, 0.65f)] public float tipResidualWidth = 0.40f;
        [Tooltip("起笔淡出长度相对于收笔淡出长度的比例；小于 1 时起笔更短、更像快速落笔。")]
        [Range(0.35f, 1f)] public float entryLengthRatio = 0.65f;
        [Tooltip("外围辉光相对线芯提前渐隐的程度，避免端部留下亮而尖的针状辉光。")]
        [Range(0f, 0.60f)] public float glowFadeAdvance = 0.25f;
        [Tooltip("真实首尾的低透明干笔短纤维存在感；0 关闭，每个端点硬限制最多 2 条，不增加材质或 Draw Call。")]
        [Range(0f, 1f)] public float dryBrushFibers = 0.15f;
        [Tooltip("悬停进入瞬间生成的短寿命墨点数量；0 可关闭。墨点消失后不会保留痕迹。")]
        [Range(0, 30)] public int splatterCount = 10;
        [Tooltip("墨点的基础半径像素，算法会对每颗做受限随机变化。")]
        [Range(0.5f, 6f)] public float splatterSize = 2.4f;
        [Tooltip("墨点相对划线附近沿法线方向扩散的范围像素；不改变按钮真实命中区。")]
        [Range(4f, 42f)] public float splatterSpread = 18f;
        [Header("Parallel only")]
        [Tooltip("仅多线并发：控制路径的手绘横向扰动，越大越不规则。SingleStroke 完全忽略。")]
        [Range(0f, 1f)] public float wobble = 0.58f;
        [Tooltip("仅多线并发：控制回勾与重叠倾向。SingleStroke 完全忽略，因而不会产生竖向回环。")]
        [Range(0f, 1f)] public float loopiness = 0.72f;
        [Header("Single stroke only")]
        [Tooltip("仅一笔往返：每一段直线允许的最大倾斜角。0 时全部水平穿过按钮中心；算法不会生成竖线。")]
        [Range(0f, 12f)] public float turnAngleDeg = 7f;
        [Header("Shared style")]
        [Tooltip("霓虹外围辉光宽度像素。线芯保持原色，辉光使用独立半透明几何，不做整层 Screen 混色。")]
        [Range(0f, 30f)] public float glow = 10f;
        [Tooltip("约 44% 笔触使用的主霓虹色。标准预设为青色。")]
        public Color primaryColor = new Color32(37, 238, 240, 255);
        [Tooltip("约 30% 笔触使用的副霓虹色。标准预设为紫色。")]
        public Color secondaryColor = new Color32(193, 37, 231, 255);
        [Tooltip("约 14% 笔触使用的高光色。标准预设为暖白；剩余约 12% 来自代码内固定补充霓虹色池。")]
        public Color accentColor = new Color32(244, 240, 234, 255);

        public const int SampleCount = 64;
        public float DrawDurationSeconds => drawDurationMs * 0.001f;

        public void ApplyWebJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            MenuScribbleRootJson root = JsonUtility.FromJson<MenuScribbleRootJson>(json);
            if (root == null || root.menuScribble == null)
            {
                return;
            }

            MenuScribbleJson data = root.menuScribble;
            if (string.Equals(data.drawMode, "singleStroke", StringComparison.OrdinalIgnoreCase))
            {
                drawMode = MenuScribbleDrawMode.SingleStroke;
            }
            else if (string.Equals(data.drawMode, "parallel", StringComparison.OrdinalIgnoreCase))
            {
                drawMode = MenuScribbleDrawMode.Parallel;
            }
            minRegionWidth = data.minRegionWidth > 0f ? data.minRegionWidth : data.regionWidth;
            leftOverflow = data.leftOverflow > 0f ? data.leftOverflow : leftOverflow;
            rightOverflow = data.rightOverflow > 0f ? data.rightOverflow : rightOverflow;
            verticalPadding = data.verticalPadding >= 0f ? data.verticalPadding : verticalPadding;
            regionHeight = data.regionHeight;
            lineCount = data.lineCount;
            drawDurationMs = data.drawDurationMs;
            thickness = data.thickness;
            bool supportsPressureAndSplatter = !string.IsNullOrEmpty(data.algorithm)
                && (data.algorithm.IndexOf("v7", StringComparison.OrdinalIgnoreCase) >= 0
                    || data.algorithm.IndexOf("v8", StringComparison.OrdinalIgnoreCase) >= 0
                    || data.algorithm.IndexOf("v9", StringComparison.OrdinalIgnoreCase) >= 0
                    || data.algorithm.IndexOf("v10", StringComparison.OrdinalIgnoreCase) >= 0
                    || string.Equals(data.algorithm, AlgorithmVersion, StringComparison.Ordinal));
            if (supportsPressureAndSplatter)
            {
                pressureVariation = data.pressureVariation;
                splatterCount = data.splatterCount;
                splatterSize = data.splatterSize;
                splatterSpread = data.splatterSpread;
            }
            bool supportsSoftLift = !string.IsNullOrEmpty(data.algorithm)
                && (data.algorithm.IndexOf("v10", StringComparison.OrdinalIgnoreCase) >= 0
                    || string.Equals(data.algorithm, AlgorithmVersion, StringComparison.Ordinal));
            if (supportsSoftLift)
            {
                tipResidualWidth = data.tipResidualWidth;
                entryLengthRatio = data.entryLengthRatio;
                glowFadeAdvance = data.glowFadeAdvance;
                dryBrushFibers = data.dryBrushFibers;
            }
            wobble = data.wobble;
            loopiness = data.loopiness;
            bool supportsZeroTurnAngle = !string.IsNullOrEmpty(data.algorithm)
                && (data.algorithm.EndsWith("v4", StringComparison.OrdinalIgnoreCase)
                    || data.algorithm.IndexOf("v5", StringComparison.OrdinalIgnoreCase) >= 0
                    || data.algorithm.IndexOf("v6", StringComparison.OrdinalIgnoreCase) >= 0
                     || data.algorithm.IndexOf("v7", StringComparison.OrdinalIgnoreCase) >= 0
                     || data.algorithm.IndexOf("v8", StringComparison.OrdinalIgnoreCase) >= 0
                     || data.algorithm.IndexOf("v9", StringComparison.OrdinalIgnoreCase) >= 0
                     || data.algorithm.IndexOf("v10", StringComparison.OrdinalIgnoreCase) >= 0
                     || string.Equals(data.algorithm, AlgorithmVersion, StringComparison.Ordinal));
            if (data.turnAngleDeg > 0f || supportsZeroTurnAngle)
            {
                turnAngleDeg = data.turnAngleDeg;
            }
            glow = data.glow;
            TryApplyColor(data.primaryColor, ref primaryColor);
            TryApplyColor(data.secondaryColor, ref secondaryColor);
            TryApplyColor(data.accentColor, ref accentColor);
            ClampValues();
        }

        private static void TryApplyColor(string html, ref Color target)
        {
            if (!string.IsNullOrEmpty(html) && ColorUtility.TryParseHtmlString(html, out Color parsed))
            {
                target = parsed;
            }
        }

        private void OnValidate()
        {
            ClampValues();
        }

        private void ClampValues()
        {
            minRegionWidth = Mathf.Clamp(minRegionWidth, 120f, 520f);
            leftOverflow = Mathf.Clamp(leftOverflow, 20f, 220f);
            rightOverflow = Mathf.Clamp(rightOverflow, 20f, 220f);
            verticalPadding = Mathf.Clamp(verticalPadding, 0f, 70f);
            regionHeight = Mathf.Clamp(regionHeight, 44f, 180f);
            lineCount = Mathf.Clamp(lineCount, 2, 16);
            drawDurationMs = Mathf.Clamp(drawDurationMs, 80f, 900f);
            thickness = Mathf.Clamp(thickness, 1f, 9f);
            pressureVariation = Mathf.Clamp01(pressureVariation);
            tipResidualWidth = Mathf.Clamp(tipResidualWidth, 0.15f, 0.65f);
            entryLengthRatio = Mathf.Clamp(entryLengthRatio, 0.35f, 1f);
            glowFadeAdvance = Mathf.Clamp(glowFadeAdvance, 0f, 0.60f);
            dryBrushFibers = Mathf.Clamp01(dryBrushFibers);
            splatterCount = Mathf.Clamp(splatterCount, 0, 30);
            splatterSize = Mathf.Clamp(splatterSize, 0.5f, 6f);
            splatterSpread = Mathf.Clamp(splatterSpread, 4f, 42f);
            wobble = Mathf.Clamp01(wobble);
            loopiness = Mathf.Clamp01(loopiness);
            turnAngleDeg = Mathf.Clamp(turnAngleDeg, 0f, 12f);
            glow = Mathf.Clamp(glow, 0f, 30f);
        }
    }

    [Serializable]
    public sealed class MenuScribbleRootJson
    {
        public MenuScribbleJson menuScribble;
    }

    [Serializable]
    public sealed class MenuScribbleJson
    {
        public string drawMode;
        public float minRegionWidth;
        public float leftOverflow;
        public float rightOverflow;
        public float verticalPadding = -1f;
        // v1 compatibility only. New exports use minRegionWidth.
        public float regionWidth;
        public float regionHeight;
        public int lineCount;
        public float drawDurationMs;
        public float thickness;
        public float pressureVariation;
        public float tipResidualWidth;
        public float entryLengthRatio;
        public float glowFadeAdvance;
        public float dryBrushFibers;
        public int splatterCount;
        public float splatterSize;
        public float splatterSpread;
        public float wobble;
        public float loopiness;
        public float turnAngleDeg;
        public float glow;
        public string primaryColor;
        public string secondaryColor;
        public string accentColor;
        public int sampleCount;
        public int singleStrokePointCount;
        public string algorithm;
    }
}
