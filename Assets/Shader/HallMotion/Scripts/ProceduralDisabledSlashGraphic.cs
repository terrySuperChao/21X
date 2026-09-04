using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Miscalculation.HallMotion
{
    /// <summary>
    /// Programmatic disabled-state strike. It sizes itself from aggregate rendered text bounds,
    /// so localized strings and font changes need no authored replacement sprite.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProceduralDisabledSlashGraphic : MaskableGraphic
    {
        [SerializeField] private TMP_Text textTarget;
        [SerializeField] private RectTransform textBoundsFallback;
        [SerializeField] private int seed = 1997;
        [Range(10f, 80f)] [SerializeField] private float sideOverflow = 32f;
        [Range(20f, 80f)] [SerializeField] private float regionHeight = 38f;
        [Range(4f, 20f)] [SerializeField] private float angleDegrees = 12f;
        [Range(0.5f, 5f)] [SerializeField] private float primaryThickness = 2.4f;
        [SerializeField] private Color primaryColor = new Color(0.93f, 0.91f, 0.90f, 0.48f);
        [SerializeField] private Color secondaryColor = new Color(0.49f, 0.46f, 0.54f, 0.42f);

        private float width;
        private float height;
        private float jitterA;
        private float jitterB;
        private float phaseA;
        private float phaseB;
        private bool requestedVisible;
        private bool refreshingLayout;
        private bool warnedInvalidBinding;
        private bool warnedInvalidScale;
        private Coroutine layoutRefreshRoutine;

        /// <summary>当前删除线使用的精确 TMP 边界，供接入校验器只读检查。</summary>
        public TMP_Text TextTarget => textTarget;

        /// <summary>没有 TMP 时显式配置的边界回退；两者都为空时删除线不会绘制。</summary>
        public RectTransform TextBoundsFallback => textBoundsFallback;

        /// <summary>当前业务禁用状态已请求显示，且 Graphic 没有因非法绑定被安全剔除。</summary>
        public bool IsVisuallyVisible => requestedVisible && !canvasRenderer.cull;

        protected override void Awake()
        {
            base.Awake();
            raycastTarget = false;
            color = Color.white;
            requestedVisible = false;
            canvasRenderer.cull = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RefreshLayout();
        }

        protected override void OnDisable()
        {
            if (layoutRefreshRoutine != null)
            {
                StopCoroutine(layoutRefreshRoutine);
                layoutRefreshRoutine = null;
            }
            base.OnDisable();
        }

        public void SetDisabledVisible(bool value)
        {
            requestedVisible = value;
            if (!value)
            {
                canvasRenderer.cull = true;
            }
            if (value)
            {
                RefreshLayout();
            }
        }

        public void BindText(TMP_Text target)
        {
            textTarget = target;
            textBoundsFallback = target != null ? target.rectTransform : null;
            RefreshLayout();
        }

        public void RefreshLayout()
        {
            if (!Application.isPlaying)
            {
                RefreshLayoutImmediately();
                return;
            }

            if (!isActiveAndEnabled || layoutRefreshRoutine != null)
            {
                return;
            }

            layoutRefreshRoutine = StartCoroutine(DeferredRefreshLayout());
        }

        private IEnumerator DeferredRefreshLayout()
        {
            yield return null;
            while (CanvasUpdateRegistry.IsRebuildingLayout() || CanvasUpdateRegistry.IsRebuildingGraphics())
            {
                yield return null;
            }

            layoutRefreshRoutine = null;
            RefreshLayoutImmediately();
        }

        /// <summary>
        /// 只允许由 MenuScribbleHover 的下一帧合并刷新或本组件自己的延迟队列调用。
        /// 返回 false 表示输入绑定/坐标非法，当前删除线会安全隐藏。
        /// </summary>
        internal bool RefreshLayoutImmediately()
        {
            if (refreshingLayout)
            {
                return false;
            }

            if (CanvasUpdateRegistry.IsRebuildingLayout() || CanvasUpdateRegistry.IsRebuildingGraphics())
            {
                if (Application.isPlaying)
                {
                    RefreshLayout();
                }
                return false;
            }

            refreshingLayout = true;
            try
            {
            RectTransform parentRect = rectTransform.parent as RectTransform;
            if (parentRect == null || !HallMotionRuntimeGuards.HasUsableScale(parentRect))
            {
                width = 0f;
                height = 0f;
                canvasRenderer.cull = true;
                if (!warnedInvalidScale)
                {
                    warnedInvalidScale = true;
                    Debug.LogError("DisabledSlash parent has zero/non-finite scale. Restore its Canvas/visual hierarchy to a valid scale (normally 1,1,1).", this);
                }
                return false;
            }

            float textWidth;
            Vector3 centerWorld;
            if (textTarget != null)
            {
                if (string.IsNullOrEmpty(textTarget.text)
                    || !HallMotionRuntimeGuards.HasUsableScale(textTarget.rectTransform))
                {
                    return HideForInvalidBinding("DisabledSlash Text Target is empty or has invalid scale; the line was hidden instead of using an arbitrary fallback size.");
                }
                textTarget.ForceMeshUpdate();
                Bounds bounds = textTarget.textBounds;
                if (!HallMotionRuntimeGuards.IsFinite(bounds) || bounds.size.x <= 0.01f)
                {
                    return HideForInvalidBinding("DisabledSlash Text Target produced empty/non-finite bounds; the line was hidden.");
                }
                textWidth = bounds.size.x;
                centerWorld = textTarget.rectTransform.TransformPoint(bounds.center);
            }
            else if (textBoundsFallback != null)
            {
                if (!HallMotionRuntimeGuards.HasUsableScale(textBoundsFallback)
                    || !HallMotionRuntimeGuards.IsFinite(textBoundsFallback.rect.width)
                    || textBoundsFallback.rect.width <= 0.01f)
                {
                    return HideForInvalidBinding("DisabledSlash fallback RectTransform is empty or has invalid scale; the line was hidden.");
                }
                textWidth = textBoundsFallback.rect.width;
                centerWorld = textBoundsFallback.TransformPoint(textBoundsFallback.rect.center);
            }
            else
            {
                return HideForInvalidBinding("DisabledSlash requires Text Target or Text Bounds Fallback; the line was hidden.");
            }

            width = textWidth + sideOverflow * 2f;
            height = regionHeight;
            if (!HallMotionRuntimeGuards.IsFinite(width)
                || !HallMotionRuntimeGuards.IsFinite(height)
                || !HallMotionRuntimeGuards.IsFinite(centerWorld)
                || width <= 0.01f
                || height <= 0.01f)
            {
                return HideForInvalidBinding("DisabledSlash calculated non-finite geometry; the line was hidden.");
            }

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
            rectTransform.position = centerWorld;

            XorShift32 random = new XorShift32(unchecked((uint)seed));
            jitterA = ((float)random.Next() - 0.5f) * 4f;
            jitterB = ((float)random.Next() - 0.5f) * 4f;
            phaseA = (float)random.Next() * Mathf.PI * 2f;
            phaseB = (float)random.Next() * Mathf.PI * 2f;
            warnedInvalidBinding = false;
            warnedInvalidScale = false;
            canvasRenderer.cull = !requestedVisible;
            SetVerticesDirty();
            return true;
            }
            finally
            {
                refreshingLayout = false;
            }
        }

        private bool HideForInvalidBinding(string message)
        {
            width = 0f;
            height = 0f;
            canvasRenderer.cull = true;
            if (!warnedInvalidBinding)
            {
                warnedInvalidBinding = true;
                Debug.LogWarning(message, this);
            }
            return false;
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
            if (!HallMotionRuntimeGuards.IsFinite(width)
                || !HallMotionRuntimeGuards.IsFinite(height)
                || width <= 0f
                || height <= 0f)
            {
                return;
            }

            float halfWidth = width * 0.5f - 6f;
            float rise = Mathf.Tan(angleDegrees * Mathf.Deg2Rad) * halfWidth * 2f * 0.5f;
            AddPressureLine(vertexHelper,
                new Vector2(-halfWidth, -rise + jitterA),
                new Vector2(halfWidth, rise - jitterA),
                primaryThickness,
                primaryColor,
                phaseA);
            AddPressureLine(vertexHelper,
                new Vector2(-halfWidth, -rise + 3f + jitterB),
                new Vector2(halfWidth, rise + 3f - jitterB),
                Mathf.Max(0.5f, primaryThickness * 0.46f),
                secondaryColor,
                phaseB);
        }

        private static void AddPressureLine(VertexHelper helper, Vector2 start, Vector2 end, float thickness, Color lineColor, float phase)
        {
            if (!HallMotionRuntimeGuards.IsFinite(start)
                || !HallMotionRuntimeGuards.IsFinite(end)
                || !HallMotionRuntimeGuards.IsFinite(thickness)
                || thickness <= 0f)
            {
                return;
            }

            const int segmentCount = 48;
            Vector2 direction = (end - start).normalized;
            if (direction.sqrMagnitude < 0.001f) direction = Vector2.right;
            Vector2 unitNormal = new Vector2(-direction.y, direction.x);
            int baseIndex = helper.currentVertCount;
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = lineColor;
            for (int i = 0; i <= segmentCount; i++)
            {
                float t = i / (float)segmentCount;
                float startTaper = SmoothStep01(Mathf.Min(1f, t / 0.12f));
                float endTaper = SmoothStep01(Mathf.Min(1f, (1f - t) / 0.12f));
                float taper = Mathf.Min(startTaper, endTaper);
                float safeEnvelope = HallMotionRuntimeGuards.NonNegativeSin(Mathf.PI * t);
                safeEnvelope *= safeEnvelope;
                float pressure = taper * (1f + 0.06f * Mathf.Sin(t * Mathf.PI * 3f + phase) * safeEnvelope);
                float wobble = Mathf.Sin(t * Mathf.PI * 3f + phase) * 0.52f * safeEnvelope;
                Vector2 point = Vector2.Lerp(start, end, t) + unitNormal * wobble;
                Vector2 normal = unitNormal * (thickness * pressure * 0.5f);
                if (!HallMotionRuntimeGuards.IsFinite(point) || !HallMotionRuntimeGuards.IsFinite(normal))
                {
                    return;
                }
                vertex.position = point + normal;
                // NeonScribbleUI 的旧式条带 AA 使用 uv.y 表示横截面位置：
                // 两侧分别必须是 0/1，光栅插值后条带中心才会得到完整覆盖。
                // 如果保留 UIVertex.simpleVert 的 (0,0)，整个删除线都会被 Shader
                // 误判为外缘而变成完全透明。uv.z 保持 0，继续走条带 AA，
                // 不影响 v1.0.9 霓虹线与墨点使用的显式覆盖路径。
                vertex.uv0 = new Vector4(t, 0f, 0f, 0f);
                helper.AddVert(vertex);
                vertex.position = point - normal;
                vertex.uv0 = new Vector4(t, 1f, 0f, 0f);
                helper.AddVert(vertex);
            }
            for (int i = 0; i < segmentCount; i++)
            {
                int startIndex = baseIndex + i * 2;
                helper.AddTriangle(startIndex, startIndex + 2, startIndex + 1);
                helper.AddTriangle(startIndex + 2, startIndex + 3, startIndex + 1);
            }
        }

        private static float SmoothStep01(float value)
        {
            float clamped = Mathf.Clamp01(value);
            return clamped * clamped * (3f - 2f * clamped);
        }

        private struct XorShift32
        {
            private uint value;

            public XorShift32(uint seedValue)
            {
                value = seedValue == 0u ? 0x6d2b79f5u : seedValue;
            }

            public double Next()
            {
                uint x = value;
                x ^= x << 13;
                x ^= x >> 17;
                x ^= x << 5;
                value = x;
                return value / 4294967296.0;
            }
        }
    }
}
