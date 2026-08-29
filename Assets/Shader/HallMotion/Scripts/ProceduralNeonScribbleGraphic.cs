using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Miscalculation.HallMotion
{
    /// <summary>
    /// 与网页 scribble.js 同构的程序化霓虹 uGUI 网格。
    ///
    /// 将它放在按钮文字后方，并把 Text Target 绑定到该按钮的 TMP_Text。组件只读取整段
    /// 文字的渲染边界来决定效果矩形，不读取字符、字形轮廓、字体名称或语言，因此中文、
    /// 英文和后续本地化可共用同一个组件与 Settings。
    ///
    /// 线芯、辉光、黑白反差描边、短寿命墨点和端部干笔纤维都合并进同一个 uGUI Graphic
    /// 网格；不要为这些层创建额外 GameObject/Material。Graphic 永远关闭 Raycast Target，
    /// 鼠标命中只由父级 Button 的固定 Graphic 决定。
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProceduralNeonScribbleGraphic : MaskableGraphic
    {
        [Tooltip("路径、区域、笔触、颜色与墨点参数。所有菜单按钮建议共用同一份资产。")]
        [SerializeField] private MenuScribbleSettings settings;
        [Tooltip("读取 hoverTextScale 和辉光呼吸参数；应与父级 MenuScribbleHover 使用同一份资产。")]
        [SerializeField] private MenuInteractionSettings interactionSettings;
        [Tooltip("首选的整段文字边界来源。绑定当前按钮 TMP_Text 后可自动适配本地化和字体变化。")]
        [SerializeField] private TMP_Text textTarget;
        [FormerlySerializedAs("horizontalAnchorTarget")]
        [Tooltip("未绑定 TMP_Text 时的兼容回退，只能读取 RectTransform 矩形，无法得到精确字形渲染宽度。正式菜单应优先使用 Text Target。")]
        [SerializeField] private RectTransform textBoundsFallback;
        [Tooltip("只在非运行状态显示固定 Seed 预览，方便 Prefab 排版；不影响运行时随机路径。")]
        [SerializeField] private bool previewInEditor;
        [Tooltip("编辑器预览路径的固定随机种子。运行时由 MenuScribbleHover 在每次真正进入按钮时传入新 Seed。")]
        [SerializeField] private uint editorPreviewSeed = 19972018;

        private static readonly Color[] ExtraNeonColors =
        {
            new Color32(59, 130, 255, 255),
            new Color32(183, 255, 60, 255),
            new Color32(255, 79, 216, 255),
            new Color32(255, 194, 71, 255)
        };

        private readonly List<StrokeData> strokes = new List<StrokeData>(16);
        private readonly List<SplatterData> splatters = new List<SplatterData>(30);
        private float animationStart;
        private bool animating;
        private bool instant;
        private bool visible;
        private bool monochrome;
        private float currentWidth;
        private float currentHeight;
        private float settledAt;
        private float animationDurationSeconds;
        private uint currentSeed;
        private float collapseProgress = -1f;
        private Coroutine layoutRefreshRoutine;
        private bool queuedRegeneratePath;
        private bool refreshingLayout;
        private bool warnedInvalidLayout;

        /// <summary>当前路径配置资产，仅供接入检查或工具读取。</summary>
        public MenuScribbleSettings Settings => settings;

        /// <summary>当前交互配置资产，仅供接入检查或工具读取。</summary>
        public MenuInteractionSettings InteractionSettings => interactionSettings;

        /// <summary>当前精确文字目标，供接入校验器只读检查。</summary>
        public TMP_Text TextTarget => textTarget;

        /// <summary>仅在无法使用 TMP 时的显式矩形回退，正式菜单不建议依赖。</summary>
        public RectTransform TextBoundsFallback => textBoundsFallback;

        /// <summary>是否已有一条可复用的可见路径；按下态据此避免首次点击重新随机。</summary>
        public bool HasVisiblePath => visible && strokes.Count > 0;

        /// <summary>当前自适应特效区域，供自动化回归与接入诊断读取。</summary>
        public Vector2 CurrentRegionSize => new Vector2(currentWidth, currentHeight);

        /// <summary>当前缓存的主笔触数；标准配置受 Settings.lineCount 与 16 条硬上限约束。</summary>
        public int CurrentStrokeCount => strokes.Count;

        /// <summary>当前缓存的短寿命墨点数；标准配置受 30 颗硬上限约束。</summary>
        public int CurrentSplatterCount => splatters.Count;

        /// <summary>
        /// 检查当前缓存路径和墨点是否全部为有限值。正式渲染前仍会逐顶点检查；该方法用于
        /// 以大量随机 Seed 做自动化回归，避免将某个极端随机结果带进 Player。
        /// </summary>
        public bool HasFiniteGeneratedGeometry()
        {
            for (int strokeIndex = 0; strokeIndex < strokes.Count; strokeIndex++)
            {
                StrokeData stroke = strokes[strokeIndex];
                if (stroke == null
                    || stroke.points == null
                    || stroke.points.Count < 2
                    || !HallMotionRuntimeGuards.IsFinite(stroke.width)
                    || !HallMotionRuntimeGuards.IsFinite(stroke.delayMs)
                    || !HallMotionRuntimeGuards.IsFinite(stroke.durationMs))
                {
                    return false;
                }
                for (int pointIndex = 0; pointIndex < stroke.points.Count; pointIndex++)
                {
                    if (!HallMotionRuntimeGuards.IsFinite(stroke.points[pointIndex]))
                    {
                        return false;
                    }
                }
            }

            for (int splatterIndex = 0; splatterIndex < splatters.Count; splatterIndex++)
            {
                SplatterData splatter = splatters[splatterIndex];
                if (splatter == null
                    || !HallMotionRuntimeGuards.IsFinite(splatter.position)
                    || !HallMotionRuntimeGuards.IsFinite(splatter.radius)
                    || !HallMotionRuntimeGuards.IsFinite(splatter.stretch)
                    || !HallMotionRuntimeGuards.IsFinite(splatter.rotation))
                {
                    return false;
                }
            }
            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            raycastTarget = false;
            color = Color.white;
            canvasRenderer.SetAlpha(0f);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (Application.isPlaying)
            {
                RefreshLayout(false);
            }
            else
            {
                RefreshLayoutImmediately(false);
            }
            if (previewInEditor && !Application.isPlaying && settings != null)
            {
                Generate(editorPreviewSeed);
                instant = true;
                canvasRenderer.SetAlpha(1f);
                SetVerticesDirty();
            }
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

        /// <summary>由 Prefab 构建器或项目接入代码绑定当前业务按钮的 TMP Label。</summary>
        public void BindText(TMP_Text target)
        {
            textTarget = target;
            textBoundsFallback = target != null ? target.rectTransform : null;
            RefreshLayout(true);
        }

        private void Update()
        {
            if (settings == null || !visible)
            {
                return;
            }

            if (animating)
            {
                SetVerticesDirty();
                if (instant || Time.unscaledTime - animationStart >= animationDurationSeconds)
                {
                    animating = false;
                    settledAt = Time.unscaledTime;
                    SetVerticesDirty();
                }
            }
            else if (!monochrome && interactionSettings != null && interactionSettings.glowBreathAmount > 0f)
            {
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// 根据给定 Seed 生成并开始绘制一条新路径。只有真正进入新的按钮状态时调用；
        /// 按下和确认必须复用当前路径，不能再次调用本方法。
        /// </summary>
        public void BeginDraw(uint seed, bool drawInstantly = false)
        {
            if (settings == null)
            {
                Debug.LogWarning("Menu scribble settings are missing.", this);
                return;
            }

            if (!TryApplyRegionSize(false))
            {
                visible = false;
                canvasRenderer.cull = true;
                return;
            }
            currentSeed = seed;
            Generate(seed);
            instant = drawInstantly;
            monochrome = false;
            collapseProgress = -1f;
            visible = true;
            animationStart = Time.unscaledTime;
            animating = true;
            canvasRenderer.cull = false;
            canvasRenderer.SetAlpha(1f);
            SetVerticesDirty();
        }

        /// <summary>停止动画并在指定秒数内淡出；duration 为 0 时用于菜单面板关闭后的立即清理。</summary>
        public void Hide(float duration = 0.12f)
        {
            animating = false;
            visible = false;
            monochrome = false;
            collapseProgress = -1f;
            CrossFadeAlpha(0f, Mathf.Max(0f, duration), true);
        }

        /// <summary>
        /// 原地切换黑白按下态。只改变当前网格配色和反差描边，绝不重新 Generate，
        /// 因而路径、Seed、线宽和随机补充色角色都保持不变。
        /// </summary>
        public void SetMonochrome(bool value)
        {
            if (strokes.Count == 0)
            {
                return;
            }

            monochrome = value;
            collapseProgress = -1f;
            visible = true;
            canvasRenderer.cull = false;
            canvasRenderer.SetAlpha(1f);
            SetVerticesDirty();
        }

        /// <summary>
        /// 将所有路径顶点按统一进度收束到整个效果矩形的同一个中心点。
        /// value 范围 0..1；确认反馈期间会强制使用黑白配色。
        /// </summary>
        public void SetCollapseProgress(float value)
        {
            if (strokes.Count == 0) return;
            collapseProgress = Mathf.Clamp01(value);
            monochrome = true;
            visible = true;
            animating = false;
            canvasRenderer.cull = false;
            canvasRenderer.SetAlpha(1f);
            SetVerticesDirty();
        }

        /// <summary>确认反馈结束后清除收束覆盖值，并恢复彩色完成态。</summary>
        public void ClearCollapse()
        {
            collapseProgress = -1f;
            monochrome = false;
            if (visible) SetVerticesDirty();
        }

        /// <summary>
        /// 重新读取 TMP 整体渲染边界。文字/字体/语言变化后调用；仅当尺寸真的变化超过 0.5px 时，
        /// 才使用原 Seed 重算当前路径，不会引入新的随机结果。
        /// </summary>
        public void RefreshLayout(bool regenerateCurrentPath = true)
        {
            queuedRegeneratePath |= regenerateCurrentPath;
            if (!Application.isPlaying)
            {
                bool regenerate = queuedRegeneratePath;
                queuedRegeneratePath = false;
                RefreshLayoutImmediately(regenerate);
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

            bool regenerate = queuedRegeneratePath;
            queuedRegeneratePath = false;
            layoutRefreshRoutine = null;
            RefreshLayoutImmediately(regenerate);
        }

        /// <summary>
        /// 只允许由 MenuScribbleHover 的下一帧合并刷新或本组件自己的延迟队列调用。
        /// </summary>
        internal bool RefreshLayoutImmediately(bool regenerateCurrentPath = true)
        {
            if (refreshingLayout)
            {
                return false;
            }

            if (CanvasUpdateRegistry.IsRebuildingLayout() || CanvasUpdateRegistry.IsRebuildingGraphics())
            {
                if (Application.isPlaying)
                {
                    RefreshLayout(regenerateCurrentPath);
                }
                return false;
            }

            refreshingLayout = true;
            try
            {
            float previousWidth = currentWidth;
            float previousHeight = currentHeight;
            if (!TryApplyRegionSize(true))
            {
                return false;
            }
            if (regenerateCurrentPath && strokes.Count > 0 &&
                (Mathf.Abs(previousWidth - currentWidth) > 0.5f || Mathf.Abs(previousHeight - currentHeight) > 0.5f))
            {
                Generate(currentSeed != 0u ? currentSeed : editorPreviewSeed);
                SetVerticesDirty();
            }
            if (visible)
            {
                canvasRenderer.cull = false;
            }
            warnedInvalidLayout = false;
            return true;
            }
            finally
            {
                refreshingLayout = false;
            }
        }

        /// <summary>
        /// 根据悬停目标文字宽高、左右固定延伸、最小区域和上下留白更新自身 RectTransform。
        /// 此方法只改特效 Graphic，不会改父 Button 的真实命中框。
        /// </summary>
        public void ApplyRegionSize()
        {
            TryApplyRegionSize(false);
        }

        private bool TryApplyRegionSize(bool forceTextMeshUpdate)
        {
            if (settings == null)
            {
                return HideForInvalidLayout("Menu scribble settings are missing; the effect was hidden.");
            }

            if (!ResolveAdaptiveRegion(forceTextMeshUpdate, out currentWidth, out currentHeight, out Vector3 centerInParent))
            {
                return false;
            }
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentHeight);
            RectTransform parentRect = rectTransform.parent as RectTransform;
            if (parentRect != null)
            {
                rectTransform.position = parentRect.TransformPoint(centerInParent);
            }
            warnedInvalidLayout = false;
            return true;
        }

        private bool ResolveAdaptiveRegion(bool forceTextMeshUpdate, out float width, out float height, out Vector3 centerInParent)
        {
            RectTransform parentRect = rectTransform.parent as RectTransform;
            if (parentRect == null || !HallMotionRuntimeGuards.HasUsableScale(parentRect))
            {
                width = 0f;
                height = 0f;
                centerInParent = Vector3.zero;
                return HideForInvalidLayout("Menu scribble parent has zero/non-finite scale. Restore its Canvas/visual hierarchy to a valid scale (normally 1,1,1).");
            }

            float textWidth;
            float textHeight;
            Vector3 centerWorld;
            if (textTarget != null)
            {
                if (string.IsNullOrEmpty(textTarget.text)
                    || !HallMotionRuntimeGuards.HasUsableScale(textTarget.rectTransform))
                {
                    width = 0f;
                    height = 0f;
                    centerInParent = Vector3.zero;
                    return HideForInvalidLayout("Menu scribble Text Target is empty or has invalid scale; the effect was hidden while the authored Button remains usable.");
                }
                if (forceTextMeshUpdate)
                {
                    textTarget.ForceMeshUpdate();
                }
                Bounds bounds = textTarget.textBounds;
                if (!HallMotionRuntimeGuards.IsFinite(bounds)
                    || bounds.size.x <= 0.01f
                    || bounds.size.y <= 0.01f)
                {
                    width = 0f;
                    height = 0f;
                    centerInParent = Vector3.zero;
                    return HideForInvalidLayout("Menu scribble Text Target produced empty/non-finite bounds; the effect was hidden.");
                }
                textWidth = bounds.size.x;
                textHeight = bounds.size.y;
                centerWorld = textTarget.rectTransform.TransformPoint(bounds.center);
            }
            else if (textBoundsFallback != null)
            {
                if (!HallMotionRuntimeGuards.HasUsableScale(textBoundsFallback)
                    || !HallMotionRuntimeGuards.IsFinite(textBoundsFallback.rect.width)
                    || !HallMotionRuntimeGuards.IsFinite(textBoundsFallback.rect.height)
                    || textBoundsFallback.rect.width <= 0.01f
                    || textBoundsFallback.rect.height <= 0.01f)
                {
                    width = 0f;
                    height = 0f;
                    centerInParent = Vector3.zero;
                    return HideForInvalidLayout("Menu scribble fallback RectTransform is empty or has invalid scale; the effect was hidden.");
                }
                textWidth = textBoundsFallback.rect.width;
                textHeight = textBoundsFallback.rect.height;
                centerWorld = textBoundsFallback.TransformPoint(textBoundsFallback.rect.center);
            }
            else
            {
                width = 0f;
                height = 0f;
                centerInParent = Vector3.zero;
                return HideForInvalidLayout("Menu scribble requires Text Target or Text Bounds Fallback; the effect was hidden.");
            }

            // 用悬停目标尺寸计算特效范围，保证文字放大后仍被同样长度的左右笔触包住。
            float hoverScale = interactionSettings != null ? interactionSettings.hoverTextScale : 1.12f;
            float contentWidth = textWidth * hoverScale;
            float contentHeight = textHeight * hoverScale;
            float desiredWidth = contentWidth + settings.leftOverflow + settings.rightOverflow;
            width = Mathf.Max(settings.minRegionWidth, desiredWidth);
            height = Mathf.Max(settings.regionHeight, contentHeight + settings.verticalPadding * 2f);
            if (!HallMotionRuntimeGuards.IsFinite(width)
                || !HallMotionRuntimeGuards.IsFinite(height)
                || !HallMotionRuntimeGuards.IsFinite(centerWorld)
                || width <= 0.01f
                || height <= 0.01f)
            {
                centerInParent = Vector3.zero;
                return HideForInvalidLayout("Menu scribble calculated non-finite geometry; the effect was hidden.");
            }
            float extraWidth = width - desiredWidth;
            float leftExtent = settings.leftOverflow + extraWidth * 0.5f;
            float rightExtent = settings.rightOverflow + extraWidth * 0.5f;
            centerInParent = parentRect.InverseTransformPoint(centerWorld);
            if (!HallMotionRuntimeGuards.IsFinite(centerInParent))
            {
                return HideForInvalidLayout("Menu scribble coordinate conversion produced NaN/Infinity; check Canvas and parent scales.");
            }
            // 左右延伸不相等时只移动特效中心；文字和 Button 命中框保持原位置。
            centerInParent.x += (rightExtent - leftExtent) * 0.5f;
            return HallMotionRuntimeGuards.IsFinite(centerInParent)
                || HideForInvalidLayout("Menu scribble offset produced NaN/Infinity; the effect was hidden.");
        }

        private bool HideForInvalidLayout(string message)
        {
            currentWidth = 0f;
            currentHeight = 0f;
            canvasRenderer.cull = true;
            if (!warnedInvalidLayout)
            {
                warnedInvalidLayout = true;
                Debug.LogWarning(message, this);
            }
            return false;
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
            if (settings == null
                || strokes.Count == 0
                || !HallMotionRuntimeGuards.IsFinite(currentWidth)
                || !HallMotionRuntimeGuards.IsFinite(currentHeight)
                || currentWidth <= 0f
                || currentHeight <= 0f)
            {
                return;
            }

            float elapsedMs = instant ? animationDurationSeconds * 1000f + 1f : (Time.unscaledTime - animationStart) * 1000f;
            float collapse = collapseProgress >= 0f ? collapseProgress : 0f;
            AddVisibleStrips(vertexHelper, elapsedMs, true, collapse);
            if (collapseProgress < 0f && !monochrome) AddVisibleSplatters(vertexHelper, elapsedMs);
            AddVisibleStrips(vertexHelper, elapsedMs, false, collapse);
        }

        private void AddVisibleStrips(VertexHelper vertexHelper, float elapsedMs, bool glowPass, float collapse)
        {
            float breathMultiplier = 1f;
            if (!animating && !monochrome && interactionSettings != null && interactionSettings.glowBreathAmount > 0f)
            {
                float period = Mathf.Max(0.1f, interactionSettings.glowBreathPeriodSeconds);
                float phase = (Time.unscaledTime - settledAt) / period * Mathf.PI * 2f;
                breathMultiplier += interactionSettings.glowBreathAmount * (0.5f + 0.5f * Mathf.Sin(phase));
            }

            for (int i = 0; i < strokes.Count; i++)
            {
                StrokeData stroke = strokes[i];
                float raw = Mathf.Clamp01((elapsedMs - stroke.delayMs) / Mathf.Max(1f, stroke.durationMs));
                if (raw <= 0f)
                {
                    continue;
                }

                float progress = 1f - Mathf.Pow(1f - raw, 3f);
                if (progress <= 0.0001f || collapse >= 0.999f) continue;
                if (glowPass)
                {
                    Color glowColor = ResolveStrokeColor(stroke);
                    glowColor.a *= 0.12f;
                    AddPressureStrip(vertexHelper, stroke, 0f, progress, stroke.width + settings.glow * 0.28f * breathMultiplier, glowColor, collapse, 1f + settings.glowFadeAdvance);
                }
                else
                {
                    Color coreColor = ResolveStrokeColor(stroke);
                    if (monochrome)
                    {
                        float outlineWidth = stroke.width + Mathf.Max(2.4f, stroke.width * 0.8f);
                        AddPressureStrip(vertexHelper, stroke, 0f, progress, outlineWidth, ResolveStrokeOutlineColor(coreColor), collapse);
                    }
                    AddPressureStrip(vertexHelper, stroke, 0f, progress, stroke.width, coreColor, collapse);
                    if (!monochrome && settings.dryBrushFibers > 0f)
                    {
                        AddDryBrushFibers(vertexHelper, stroke, progress, stroke.width, coreColor, collapse);
                    }
                }
            }
        }

        private void AddVisibleSplatters(VertexHelper vertexHelper, float elapsedMs)
        {
            for (int i = 0; i < splatters.Count; i++)
            {
                SplatterData splatter = splatters[i];
                float progress = (elapsedMs - splatter.delayMs) / Mathf.Max(1f, splatter.lifetimeMs);
                if (progress <= 0f || progress >= 1f) continue;
                float appear = Mathf.Min(1f, progress / 0.16f);
                float fade = 1f - Mathf.Max(0f, (progress - 0.42f) / 0.58f);
                Color splatterColor = ResolveColor(splatter.color, splatter.colorRole);
                splatterColor.a *= Mathf.Pow(Mathf.Max(0f, appear * fade), 0.72f) * 0.92f;
                float animatedRadius = splatter.radius * (0.92f + 0.08f * Mathf.Sin(progress * Mathf.PI));
                AddEllipse(vertexHelper, splatter.position, animatedRadius, splatter.stretch, splatter.rotation, splatterColor);
            }
        }

        private void Generate(uint seed)
        {
            strokes.Clear();
            splatters.Clear();
            if (!HallMotionRuntimeGuards.IsFinite(currentWidth)
                || !HallMotionRuntimeGuards.IsFinite(currentHeight)
                || currentWidth <= 0f
                || currentHeight <= 0f)
            {
                return;
            }
            XorShift32 random = new XorShift32(seed);
            float width = Mathf.Max(1f, currentWidth);
            float height = Mathf.Max(1f, currentHeight);

            if (settings.drawMode == MenuScribbleDrawMode.SingleStroke)
            {
                GenerateSingleStroke(ref random, width, height);
            }
            else
            {
                GenerateParallel(ref random, width, height);
            }
            GenerateSplatters(seed);
            float latestEndMs = settings.drawDurationMs * 1.18f;
            for (int i = 0; i < strokes.Count; i++) latestEndMs = Mathf.Max(latestEndMs, strokes[i].delayMs + strokes[i].durationMs);
            for (int i = 0; i < splatters.Count; i++) latestEndMs = Mathf.Max(latestEndMs, splatters[i].delayMs + splatters[i].lifetimeMs);
            animationDurationSeconds = latestEndMs * 0.001f;
        }

        private Color PickColor(ref XorShift32 random, out StrokeColorRole role)
        {
            double roll = random.Next();
            if (roll < 0.44)
            {
                role = StrokeColorRole.Primary;
                return settings.primaryColor;
            }
            if (roll < 0.76)
            {
                role = StrokeColorRole.Secondary;
                return settings.secondaryColor;
            }
            if (roll < 0.88)
            {
                role = StrokeColorRole.Accent;
                return settings.accentColor;
            }
            role = StrokeColorRole.Extra;
            int extraIndex = Mathf.Min(ExtraNeonColors.Length - 1, Mathf.FloorToInt((float)random.Next() * ExtraNeonColors.Length));
            return ExtraNeonColors[extraIndex];
        }

        private Color ResolveStrokeColor(StrokeData stroke)
        {
            return ResolveColor(stroke.color, stroke.colorRole);
        }

        private Color ResolveColor(Color sourceColor, StrokeColorRole role)
        {
            if (!monochrome) return sourceColor;
            return role == StrokeColorRole.Secondary || role == StrokeColorRole.Accent
                ? new Color32(244, 240, 234, 255)
                : new Color32(5, 4, 7, 255);
        }

        private static Color ResolveStrokeOutlineColor(Color coreColor)
        {
            return coreColor.grayscale < 0.5f
                ? new Color32(244, 240, 234, 245)
                : new Color32(5, 4, 7, 245);
        }

        private void GenerateParallel(ref XorShift32 random, float width, float height)
        {

            for (int lineIndex = 0; lineIndex < settings.lineCount; lineIndex++)
            {
                float delayMs = settings.drawDurationMs * (0.02f + (float)random.Next() * 0.16f);
                float durationMs = settings.drawDurationMs * (0.68f + (float)random.Next() * 0.30f);
                Color strokeColor = PickColor(ref random, out StrokeColorRole colorRole);
                float lineWidth = settings.thickness * (0.72f + (float)random.Next() * 0.56f);
                StrokeData stroke = new StrokeData
                {
                    delayMs = delayMs,
                    durationMs = durationMs,
                    color = strokeColor,
                    colorRole = colorRole,
                    width = lineWidth,
                    taperStart = true,
                    taperEnd = true,
                    points = new List<Vector2>(MenuScribbleSettings.SampleCount)
                };

                int direction = random.Next() < 0.18 ? -1 : 1;
                float turns = 0.58f + (float)random.Next() * (0.55f + 0.85f * settings.loopiness);
                float phase = (float)random.Next() * Mathf.PI * 2f;
                float phase2 = (float)random.Next() * Mathf.PI * 2f;
                float center = height * (0.5f + ((float)random.Next() - 0.5f) * 0.22f);
                float verticalAmp = height * (0.10f + (float)random.Next() * 0.20f) * (0.25f + 0.75f * settings.loopiness);
                float horizontalAmp = width * (0.04f + (float)random.Next() * 0.11f) * settings.loopiness;
                float bend = ((float)random.Next() - 0.5f) * height * 0.28f * settings.wobble;
                float flutterAmp = height * (0.015f + (float)random.Next() * 0.06f) * settings.wobble;
                float flutterFrequency = 1.8f + (float)random.Next() * 3.2f;
                float startInset = width * (0.015f + (float)random.Next() * 0.07f);
                float endInset = width * (0.015f + (float)random.Next() * 0.07f);

                for (int pointIndex = 0; pointIndex < MenuScribbleSettings.SampleCount; pointIndex++)
                {
                    float t = pointIndex / (MenuScribbleSettings.SampleCount - 1f);
                    float u = direction > 0 ? t : 1f - t;
                    float envelope = Mathf.Pow(HallMotionRuntimeGuards.NonNegativeSin(Mathf.PI * u), 0.45f);
                    float x = startInset + (width - startInset - endInset) * u
                        + Mathf.Sin(u * Mathf.PI * 2f * turns + phase) * horizontalAmp * envelope;
                    float y = center
                        + Mathf.Cos(u * Mathf.PI * 2f * turns + phase) * verticalAmp * envelope
                        + Mathf.Sin(Mathf.PI * u) * bend
                        + Mathf.Sin(u * Mathf.PI * 2f * flutterFrequency + phase2) * flutterAmp * envelope;
                    Vector2 point = new Vector2(x - width * 0.5f, height * 0.5f - y);
                    if (!HallMotionRuntimeGuards.IsFinite(point))
                    {
                        stroke.points.Clear();
                        break;
                    }
                    stroke.points.Add(point);
                }

                stroke.pressurePhase = (float)random.Next() * Mathf.PI * 2f;
                stroke.pressureFrequency = 1.25f + (float)random.Next() * 2.25f;

                if (stroke.points.Count >= 2)
                {
                    strokes.Add(stroke);
                }
            }
        }

        private void GenerateSingleStroke(ref XorShift32 random, float width, float height)
        {
            int passCount = Mathf.Max(2, settings.lineCount);
            float startDelayMs = settings.drawDurationMs * 0.02f;
            float passDurationMs = settings.drawDurationMs * 0.96f / passCount;
            float leftMin = width * 0.02f;
            float leftMax = width * 0.16f;
            float rightMin = width * 0.84f;
            float rightMax = width * 0.98f;
            float maxAngleRadians = settings.turnAngleDeg * Mathf.Deg2Rad;
            float minimumHorizontalSpan = rightMin - leftMax;
            float angleReach = Mathf.Tan(maxAngleRadians) * minimumHorizontalSpan;
            float activeHalfHeight = Mathf.Min(height * 0.36f, Mathf.Max(0f, angleReach));
            float centerY = height * 0.5f;
            float activeTop = centerY - activeHalfHeight;
            float activeBottom = centerY + activeHalfHeight;
            Vector2 previousEnd = new Vector2(
                leftMin + (float)random.Next() * (leftMax - leftMin),
                activeTop
            );

            for (int passIndex = 0; passIndex < passCount; passIndex++)
            {
                bool movingRight = passIndex % 2 == 0;
                float targetX = movingRight
                    ? rightMin + (float)random.Next() * (rightMax - rightMin)
                    : leftMin + (float)random.Next() * (leftMax - leftMin);
                float maxDeltaY = Mathf.Tan(maxAngleRadians) * Mathf.Abs(targetX - previousEnd.x);
                float allowedTop = Mathf.Max(activeTop, previousEnd.y - maxDeltaY);
                float allowedBottom = Mathf.Min(activeBottom, previousEnd.y + maxDeltaY);
                float desiredTargetY = passIndex < 2
                    ? centerY + ((float)random.Next() - 0.5f) * activeHalfHeight * 0.24f
                    : centerY + ((float)random.Next() + (float)random.Next() - 1f) * activeHalfHeight;
                float targetY = Mathf.Clamp(desiredTargetY, allowedTop, allowedBottom);
                Color strokeColor = PickColor(ref random, out StrokeColorRole colorRole);
                StrokeData stroke = new StrokeData
                {
                    delayMs = startDelayMs + passIndex * passDurationMs,
                    durationMs = passDurationMs,
                    color = strokeColor,
                    colorRole = colorRole,
                    width = settings.thickness * (0.82f + (float)random.Next() * 0.36f),
                    taperStart = passIndex == 0,
                    taperEnd = passIndex == passCount - 1,
                    points = new List<Vector2>(2)
                };
                stroke.points.Add(new Vector2(previousEnd.x - width * 0.5f, height * 0.5f - previousEnd.y));
                stroke.points.Add(new Vector2(targetX - width * 0.5f, height * 0.5f - targetY));
                stroke.pressurePhase = (float)random.Next() * Mathf.PI * 2f;
                stroke.pressureFrequency = 1.25f + (float)random.Next() * 2.25f;
                previousEnd = new Vector2(targetX, targetY);
                if (HallMotionRuntimeGuards.IsFinite(stroke.points[0])
                    && HallMotionRuntimeGuards.IsFinite(stroke.points[1]))
                {
                    strokes.Add(stroke);
                }
            }
        }

        private void GenerateSplatters(uint seed)
        {
            if (strokes.Count == 0 || settings.splatterCount <= 0) return;
            XorShift32 random = new XorShift32(seed ^ 0xa511e9b3u);
            for (int i = 0; i < settings.splatterCount; i++)
            {
                StrokeData stroke = strokes[Mathf.Min(strokes.Count - 1, Mathf.FloorToInt((float)random.Next() * strokes.Count))];
                float t = 0.04f + (float)random.Next() * 0.92f;
                Vector2 point = SampleStrokePoint(stroke, t);
                Vector2 before = SampleStrokePoint(stroke, Mathf.Max(0f, t - 0.015f));
                Vector2 after = SampleStrokePoint(stroke, Mathf.Min(1f, t + 0.015f));
                Vector2 direction = (after - before).normalized;
                if (direction.sqrMagnitude < 0.001f) direction = Vector2.right;
                Vector2 normal = new Vector2(-direction.y, direction.x);
                float side = random.Next() < 0.5 ? -1f : 1f;
                float spread = settings.splatterSpread * (0.20f + (float)random.Next() * 0.80f) * side;
                float along = ((float)random.Next() - 0.5f) * settings.splatterSpread * 0.45f;
                splatters.Add(new SplatterData
                {
                    position = point + normal * spread + direction * along,
                    radius = settings.splatterSize * (0.42f + (float)random.Next() * 1.05f),
                    stretch = 1.15f + (float)random.Next() * 1.85f,
                    rotation = (float)random.Next() * Mathf.PI * 2f,
                    delayMs = settings.drawDurationMs * (0.01f + (float)random.Next() * 0.24f),
                    lifetimeMs = Mathf.Min(460f, Mathf.Max(180f, settings.drawDurationMs * (0.78f + (float)random.Next() * 0.58f))),
                    color = stroke.color,
                    colorRole = stroke.colorRole
                });
            }
        }

        private void ResolveTaperRanges(out float startRange, out float endRange)
        {
            endRange = 0.055f + settings.pressureVariation * 0.11f;
            startRange = endRange * settings.entryLengthRatio;
        }

        private float PressureAt(StrokeData stroke, float progress)
        {
            ResolveTaperRanges(out float startRange, out float endRange);
            float startTaper = stroke.taperStart ? SmoothStep01(progress / Mathf.Max(0.0001f, startRange)) : 1f;
            float endTaper = stroke.taperEnd ? SmoothStep01((1f - progress) / Mathf.Max(0.0001f, endRange)) : 1f;
            float lift = Mathf.Max(0f, Mathf.Min(startTaper, endTaper));
            return settings.tipResidualWidth + (1f - settings.tipResidualWidth) * lift;
        }

        private float TipOpacityAt(StrokeData stroke, float progress, float fadeScale = 1f)
        {
            ResolveTaperRanges(out float startRange, out float endRange);
            float startFadeRange = startRange * 0.68f * fadeScale;
            float endFadeRange = endRange * 0.62f * fadeScale;
            float startFade = stroke.taperStart
                ? Mathf.Pow(SmoothStep01(progress / Mathf.Max(0.0001f, startFadeRange)), 1.12f)
                : 1f;
            float endFade = stroke.taperEnd
                ? Mathf.Pow(SmoothStep01((1f - progress) / Mathf.Max(0.0001f, endFadeRange)), 1.12f)
                : 1f;
            return Mathf.Clamp01(Mathf.Min(startFade, endFade));
        }

        private static float SmoothStep01(float value)
        {
            float clamped = Mathf.Clamp01(value);
            return clamped * clamped * (3f - 2f * clamped);
        }

        private static Vector2 SampleStrokePoint(StrokeData stroke, float progress)
        {
            if (stroke == null || stroke.points == null || stroke.points.Count < 2)
            {
                return Vector2.zero;
            }
            float clamped = Mathf.Clamp01(progress);
            float position = clamped * (stroke.points.Count - 1);
            int index = Mathf.Min(stroke.points.Count - 2, Mathf.FloorToInt(position));
            float fraction = position - index;
            return Vector2.Lerp(stroke.points[Mathf.Max(0, index)], stroke.points[Mathf.Min(stroke.points.Count - 1, index + 1)], fraction);
        }

        private void AddPressureStrip(VertexHelper vertexHelper, StrokeData stroke, float startProgress, float endProgress, float width, Color color, float collapse, float tipFadeScale = 1f)
        {
            if (stroke == null
                || stroke.points == null
                || stroke.points.Count < 2
                || !HallMotionRuntimeGuards.IsFinite(startProgress)
                || !HallMotionRuntimeGuards.IsFinite(endProgress)
                || !HallMotionRuntimeGuards.IsFinite(width)
                || !HallMotionRuntimeGuards.IsFinite(collapse)
                || endProgress - startProgress <= 0.0001f
                || width <= 0f)
            {
                return;
            }
            float collapseScale = Mathf.Max(0f, 1f - collapse);
            if (collapseScale <= 0.001f) return;
            int sampleCount = Mathf.Max(8, Mathf.CeilToInt((endProgress - startProgress) * 96f) + 1);
            int baseVertex = vertexHelper.currentVertCount;
            UIVertex vertex = UIVertex.simpleVert;

            for (int i = 0; i < sampleCount; i++)
            {
                float local = i / Mathf.Max(1f, sampleCount - 1f);
                float progress = Mathf.Lerp(startProgress, endProgress, local);
                Vector2 point = SampleStrokePoint(stroke, progress) * collapseScale;
                Vector2 previous = SampleStrokePoint(stroke, Mathf.Max(startProgress, progress - 0.012f));
                Vector2 next = SampleStrokePoint(stroke, Mathf.Min(endProgress, progress + 0.012f));
                if (!HallMotionRuntimeGuards.IsFinite(point)
                    || !HallMotionRuntimeGuards.IsFinite(previous)
                    || !HallMotionRuntimeGuards.IsFinite(next))
                {
                    return;
                }
                Vector2 tangent = (next - previous).normalized;
                if (tangent.sqrMagnitude < 0.001f) tangent = Vector2.right;
                float halfWidth = width * PressureAt(stroke, progress) * 0.5f * collapseScale;
                Vector2 normal = new Vector2(-tangent.y, tangent.x);
                if (!HallMotionRuntimeGuards.IsFinite(halfWidth) || !HallMotionRuntimeGuards.IsFinite(normal))
                {
                    return;
                }
                Color vertexColor = color;
                vertexColor.a *= TipOpacityAt(stroke, progress, tipFadeScale);
                vertex.color = vertexColor;
                vertex.position = point + normal * halfWidth;
                vertex.uv0 = new Vector2(local, 0f);
                vertexHelper.AddVert(vertex);
                vertex.position = point - normal * halfWidth;
                vertex.uv0 = new Vector2(local, 1f);
                vertexHelper.AddVert(vertex);
            }

            for (int i = 0; i < sampleCount - 1; i++)
            {
                int start = baseVertex + i * 2;
                vertexHelper.AddTriangle(start, start + 2, start + 1);
                vertexHelper.AddTriangle(start + 2, start + 3, start + 1);
            }

            if (!stroke.taperStart && startProgress <= 0.0001f)
            {
                float radius = width * PressureAt(stroke, startProgress) * 0.5f * collapseScale;
                AddCircle(vertexHelper, SampleStrokePoint(stroke, startProgress) * collapseScale, radius, color);
            }
            if (endProgress < 0.999f || !stroke.taperEnd)
            {
                float radius = width * PressureAt(stroke, endProgress) * 0.5f * collapseScale;
                AddCircle(vertexHelper, SampleStrokePoint(stroke, endProgress) * collapseScale, radius, color);
            }
        }

        private void AddDryBrushFibers(VertexHelper vertexHelper, StrokeData stroke, float progress, float width, Color color, float collapse)
        {
            if (stroke == null || stroke.points == null || stroke.points.Count < 2)
            {
                return;
            }
            float intensity = Mathf.Clamp01(settings.dryBrushFibers);
            float collapseScale = Mathf.Max(0f, 1f - collapse);
            if (intensity <= 0.0001f || progress <= 0.0001f || collapseScale <= 0.001f) return;
            int count = Mathf.Min(2, 1 + Mathf.FloorToInt(intensity * 1.999f));
            ResolveTaperRanges(out float startRange, out float endRange);

            void AddEndpointFibers(bool isStart)
            {
                float range = isStart ? startRange : endRange;
                for (int index = 0; index < count; index++)
                {
                    float near = 0.08f + index * 0.055f;
                    float far = Mathf.Min(0.74f, 0.42f + index * 0.14f + intensity * 0.10f);
                    float t0 = isStart ? range * near : 1f - range * far;
                    float targetT1 = isStart ? range * far : 1f - range * near;
                    float t1 = Mathf.Min(progress, targetT1);
                    if (t1 - t0 <= 0.0001f) continue;

                    float midpoint = (t0 + t1) * 0.5f;
                    Vector2 before = SampleStrokePoint(stroke, Mathf.Max(0f, midpoint - 0.012f));
                    Vector2 after = SampleStrokePoint(stroke, Mathf.Min(1f, midpoint + 0.012f));
                    Vector2 direction = (after - before).normalized;
                    if (direction.sqrMagnitude < 0.001f) direction = Vector2.right;
                    Vector2 normal = new Vector2(-direction.y, direction.x);
                    float phase = stroke.pressurePhase + (isStart ? 0.73f : 2.61f) + index * 1.91f;
                    float offset0 = Mathf.Sin(phase) * width * (0.10f + intensity * 0.12f);
                    float offset1 = Mathf.Sin(phase + 0.82f) * width * (0.13f + intensity * 0.16f);
                    Vector2 point0 = (SampleStrokePoint(stroke, t0) + normal * offset0) * collapseScale;
                    Vector2 point1 = (SampleStrokePoint(stroke, t1) + normal * offset1) * collapseScale;
                    float fiberWidth = Mathf.Max(0.55f, width * (0.075f + intensity * 0.095f) * (1f - index * 0.14f)) * collapseScale;
                    float alpha = (0.18f + intensity * 0.38f) * (1f - index * 0.18f);
                    Color startColor = color;
                    Color endColor = color;
                    startColor.a *= alpha * 0.72f;
                    endColor.a *= alpha;
                    AddFiberQuad(vertexHelper, point0, point1, fiberWidth, startColor, endColor);
                }
            }

            if (stroke.taperStart) AddEndpointFibers(true);
            if (stroke.taperEnd && progress > 1f - endRange) AddEndpointFibers(false);
        }

        private static void AddFiberQuad(VertexHelper helper, Vector2 start, Vector2 end, float width, Color startColor, Color endColor)
        {
            if (!HallMotionRuntimeGuards.IsFinite(start)
                || !HallMotionRuntimeGuards.IsFinite(end)
                || !HallMotionRuntimeGuards.IsFinite(width))
            {
                return;
            }
            Vector2 direction = (end - start).normalized;
            if (direction.sqrMagnitude < 0.001f || width <= 0.01f) return;
            Vector2 normal = new Vector2(-direction.y, direction.x) * (width * 0.5f);
            int baseVertex = helper.currentVertCount;
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = startColor;
            vertex.position = start + normal;
            vertex.uv0 = new Vector2(0f, 0f);
            helper.AddVert(vertex);
            vertex.position = start - normal;
            vertex.uv0 = new Vector2(0f, 1f);
            helper.AddVert(vertex);
            vertex.color = endColor;
            vertex.position = end + normal;
            vertex.uv0 = new Vector2(1f, 0f);
            helper.AddVert(vertex);
            vertex.position = end - normal;
            vertex.uv0 = new Vector2(1f, 1f);
            helper.AddVert(vertex);
            helper.AddTriangle(baseVertex, baseVertex + 2, baseVertex + 1);
            helper.AddTriangle(baseVertex + 2, baseVertex + 3, baseVertex + 1);
        }

        private static void AddCircle(VertexHelper helper, Vector2 center, float radius, Color color)
        {
            if (!HallMotionRuntimeGuards.IsFinite(center)
                || !HallMotionRuntimeGuards.IsFinite(radius)
                || radius <= 0.01f)
            {
                return;
            }
            const int sides = 12;
            int centerIndex = helper.currentVertCount;
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;
            vertex.position = center;
            helper.AddVert(vertex);
            for (int i = 0; i <= sides; i++)
            {
                float angle = i / (float)sides * Mathf.PI * 2f;
                vertex.position = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                helper.AddVert(vertex);
            }
            for (int i = 0; i < sides; i++) helper.AddTriangle(centerIndex, centerIndex + i + 1, centerIndex + i + 2);
        }

        private static void AddEllipse(VertexHelper helper, Vector2 center, float radius, float stretch, float rotation, Color color)
        {
            if (!HallMotionRuntimeGuards.IsFinite(center)
                || !HallMotionRuntimeGuards.IsFinite(radius)
                || !HallMotionRuntimeGuards.IsFinite(stretch)
                || !HallMotionRuntimeGuards.IsFinite(rotation)
                || radius <= 0f
                || stretch <= 0f)
            {
                return;
            }
            const int sides = 10;
            int centerIndex = helper.currentVertCount;
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;
            vertex.position = center;
            helper.AddVert(vertex);
            float cos = Mathf.Cos(rotation);
            float sin = Mathf.Sin(rotation);
            for (int i = 0; i <= sides; i++)
            {
                float angle = i / (float)sides * Mathf.PI * 2f;
                Vector2 local = new Vector2(Mathf.Cos(angle) * radius * stretch, Mathf.Sin(angle) * radius * 0.68f);
                vertex.position = center + new Vector2(local.x * cos - local.y * sin, local.x * sin + local.y * cos);
                helper.AddVert(vertex);
            }
            for (int i = 0; i < sides; i++) helper.AddTriangle(centerIndex, centerIndex + i + 1, centerIndex + i + 2);
        }

        private sealed class StrokeData
        {
            public float delayMs;
            public float durationMs;
            public float width;
            public Color color;
            public StrokeColorRole colorRole;
            public float pressurePhase;
            public float pressureFrequency;
            public bool taperStart;
            public bool taperEnd;
            public List<Vector2> points;
        }

        private sealed class SplatterData
        {
            public Vector2 position;
            public float radius;
            public float stretch;
            public float rotation;
            public float delayMs;
            public float lifetimeMs;
            public Color color;
            public StrokeColorRole colorRole;
        }

        private enum StrokeColorRole
        {
            Primary,
            Secondary,
            Accent,
            Extra
        }

        private struct XorShift32
        {
            private uint value;

            public XorShift32(uint seed)
            {
                value = seed == 0 ? 0x6d2b79f5u : seed;
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

        private static class ListPool<T>
        {
            private static readonly Stack<List<T>> Pool = new Stack<List<T>>();

            public static List<T> Get()
            {
                return Pool.Count > 0 ? Pool.Pop() : new List<T>(64);
            }

            public static void Release(List<T> list)
            {
                list.Clear();
                Pool.Push(list);
            }
        }
    }
}
