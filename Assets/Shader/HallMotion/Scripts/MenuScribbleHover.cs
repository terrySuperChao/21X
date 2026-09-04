using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Miscalculation.HallMotion
{
    /// <summary>
    /// 可直接加在现有 uGUI Button 上的主菜单视觉状态适配器。
    ///
    /// 业务层仍然使用原 Button.onClick、Button.interactable 和 Navigation；本组件只负责：
    /// 1. 将鼠标悬停、键盘/手柄选择统一为同一套 Hover/Focus 反馈；
    /// 2. 驱动文字放大/固定投影、霓虹划线、黑白按下态和确认收束；
    /// 3. 用整段 TMP 渲染边界计算命中宽度和特效宽度，不读取具体字符，因此可复用于多语言。
    ///
    /// 接入时必须让 InteractionVisualRoot 成为 Button 的子级视觉根节点，绝不能指向
    /// Button 自身，否则按压缩放会改变真实射线命中框，造成边缘反复 Enter/Exit。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class MenuScribbleHover : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        ISelectHandler,
        IDeselectHandler,
        ISubmitHandler
    {
        // Fixed hover shadow from the approved art reference. These values are intentionally
        // constants rather than designer parameters: distance 2px, spread 98%, size 3px,
        // angle -3 degrees, #000000 at 100% opacity.
        private const float HoverShadowDistancePixels = 2f;
        private const float HoverShadowSizePixels = 3f;
        private const float HoverShadowAngleDegrees = -3f;
        private const float HoverShadowSoftness = 0.02f;
        private const float WebPixelToTmpOutlineScale = 1.6f;

        [Header("State")]
        [Tooltip("现有业务 Button。留空时自动读取同物体上的 Button；不会替换或清空原有 onClick。")]
        [SerializeField] private Button button;
        [Tooltip("Standard 为普通按钮；Highlighted 为长期高亮类型，会保留青紫错位底光并偶发提示。它不是当前选中态。")]
        [SerializeField] private MenuButtonVariant variant = MenuButtonVariant.Standard;
        [Tooltip("悬停、按压、确认、命中留白与高亮提示的共享参数资产。所有主菜单按钮建议共用同一份。")]
        [SerializeField] private MenuInteractionSettings interactionSettings;
        [Tooltip("进入界面时立即画出霓虹线。只用于网页中类似“继续”的预先展示按钮；一般按钮保持关闭。")]
        [SerializeField] private bool drawOnStart;
        [Tooltip("减少动态效果：划线与过渡立即完成，并停止长期高亮的偶发提示。可由设置菜单调用 SetReducedMotion。")]
        [SerializeField] private bool reducedMotion;
        [Tooltip("每个按钮的随机盐值。相同帧创建多个按钮时用于避免得到相同的随机线；不影响业务逻辑。")]
        [SerializeField] private int seedSalt = 1997;

        [Header("Fixed hitbox")]
        [Tooltip("按常态 TMP 实际文字宽度 + 左右 hitPaddingX 自动设置 Button 宽度。父 LayoutGroup 若强制拉伸宽度，请关闭此项并由项目自行提供固定命中框。")]
        [SerializeField] private bool autoSizeHitboxToText = true;

        [Header("Visual references")]
        [Tooltip("按钮现有的 TextMeshProUGUI 文本。文字内容、字体和语言可以在运行时更换。")]
        [SerializeField] private TMP_Text label;
        [Tooltip("只负责悬停时放大文字的根节点。建议是 Label 外单独包的一层，不要与 InteractionVisualRoot 使用同一个 RectTransform。")]
        [SerializeField] private RectTransform labelVisualRoot;
        [Tooltip("负责按下/确认时整体缩放的独立子级视觉根节点。它应包含 Scribble 与 LabelVisualRoot，且绝不能是 Button 自己的 RectTransform。")]
        [SerializeField] private RectTransform interactionVisualRoot;
        [Tooltip("放在文字后方的 ProceduralNeonScribbleGraphic。该 Graphic 必须关闭 Raycast Target。")]
        [SerializeField] private ProceduralNeonScribbleGraphic scribble;
        [Tooltip("历史版本的禁用删除线引用。v1.0.11 起永久隐藏，仅为旧 Prefab 反序列化兼容保留。")]
        [SerializeField] private ProceduralDisabledSlashGraphic disabledSlash;

        [Header("Highlighted text echoes")]
        [Tooltip("Highlighted 类型缺少青紫文字回声时自动创建。正式 Prefab 也可手工创建后绑定下方两个字段。")]
        [SerializeField] private bool autoCreateHighlightEchoes = true;
        [Tooltip("Highlighted 常态的青色错位文字回声；可留空自动创建，不参与射线和布局。")]
        [SerializeField] private TMP_Text cyanEcho;
        [Tooltip("Highlighted 常态的紫色错位文字回声；可留空自动创建，不参与射线和布局。")]
        [SerializeField] private TMP_Text magentaEcho;
        [Tooltip("正文后方的黑色投影；可留空自动创建。它只复制整段 TMP，不读取字符或扩大命中框。")]
        [SerializeField] private TMP_Text shadowEcho;

        [Header("Events")]
        [Tooltip("每次真正生成一条新霓虹路径时触发，适合播放悬停短音；重复 Select/PointerEnter 不会重复触发。")]
        [SerializeField] private UnityEvent onScribbleDrawn;
        [Tooltip("确认反馈开始时触发，适合播放确认音或轻量逻辑。若回调会隐藏菜单/切场景，请改接 On Confirm Visual Completed。")]
        [SerializeField] private UnityEvent onConfirmed;
        [Tooltip("黑白线条收束和文字回弹完成后触发。需要完整看到确认动画的场景切换/面板跳转应接这里，并从 Button.onClick 移除同一条立即跳转回调。")]
        [SerializeField] private UnityEvent onConfirmVisualCompleted;

        private static MenuScribbleHover current;
        private System.Random random;
        private Vector3 labelBaseScale;
        private Vector3 visualBaseScale;
        private Vector2 labelVisualBasePosition;
        private Vector2 visualBasePosition;
        private Quaternion visualBaseRotation;
        private Color labelBaseColor;
        private float labelBaseOutlineWidth;
        private Color32 labelBaseOutlineColor;
        private Material labelBaseSharedMaterial;
        private Material shadowOutlineMaterial;
        private int shadowMaterialFontInstanceId;
        private int baseMaterialFontInstanceId;
        private float hoverVisualProgress;
        private float highlightPromptProgress;
        private bool warnedUnsupportedOutlineMaterial;
        private bool pointerInside;
        private bool focused;
        private bool selectedByPointer;
        private bool confirming;
        private bool previousInteractable;
        private float lastConfirmTime = -10f;
        private Coroutine hoverRoutine;
        private Coroutine pressRoutine;
        private Coroutine confirmRoutine;
        private Coroutine highlightRoutine;
        private Coroutine layoutRefreshRoutine;
        private bool hasStarted;
        private bool layoutRefreshRequested;
        private bool regeneratePathOnLayoutRefresh;
        private bool synchronizingTextAndLayout;
        private bool activateAfterInitialLayout;
        private bool warnedInvalidLabelLayout;
        private bool warnedInvalidTransformScale;
        private LayoutSignature lastLayoutSignature;
        private bool hasLayoutSignature;

        private struct LayoutSignature : IEquatable<LayoutSignature>
        {
            public string text;
            public int fontInstanceId;
            public float fontSize;
            public FontStyles fontStyle;
            public FontWeight fontWeight;
            public TextAlignmentOptions alignment;
            public float characterSpacing;
            public float wordSpacing;
            public float lineSpacing;
            public float paragraphSpacing;
            public TextOverflowModes overflowMode;
            public bool richText;
            public bool enableAutoSizing;
            public bool enableWordWrapping;
            public Vector4 margin;
            public Vector2 rectSize;

            public bool Equals(LayoutSignature other)
            {
                return string.Equals(text, other.text, StringComparison.Ordinal)
                    && fontInstanceId == other.fontInstanceId
                    && Approximately(fontSize, other.fontSize)
                    && fontStyle == other.fontStyle
                    && fontWeight == other.fontWeight
                    && alignment == other.alignment
                    && Approximately(characterSpacing, other.characterSpacing)
                    && Approximately(wordSpacing, other.wordSpacing)
                    && Approximately(lineSpacing, other.lineSpacing)
                    && Approximately(paragraphSpacing, other.paragraphSpacing)
                    && overflowMode == other.overflowMode
                    && richText == other.richText
                    && enableAutoSizing == other.enableAutoSizing
                    && enableWordWrapping == other.enableWordWrapping
                    && Approximately(margin, other.margin)
                    && Approximately(rectSize, other.rectSize);
            }

            private static bool Approximately(float a, float b)
            {
                return Mathf.Abs(a - b) <= 0.001f;
            }

            private static bool Approximately(Vector2 a, Vector2 b)
            {
                return Approximately(a.x, b.x) && Approximately(a.y, b.y);
            }

            private static bool Approximately(Vector4 a, Vector4 b)
            {
                return Approximately(a.x, b.x)
                    && Approximately(a.y, b.y)
                    && Approximately(a.z, b.z)
                    && Approximately(a.w, b.w);
            }
        }

        /// <summary>确认收束动画当前是否正在播放，可供项目输入门或页面状态机查询。</summary>
        public bool IsConfirming => confirming;

        /// <summary>当前确认视觉时长；项目若不用完成事件，也可用它作为页面跳转的最短延迟。</summary>
        public float ConfirmVisualDurationSeconds => Settings.confirmDurationSeconds;

        /// <summary>只读接入信息，供示例场景、校验器和项目侧诊断使用。</summary>
        public Button TargetButton => button;
        public TMP_Text Label => label;
        public RectTransform LabelVisualRoot => labelVisualRoot;
        public RectTransform InteractionVisualRoot => interactionVisualRoot;
        public ProceduralNeonScribbleGraphic Scribble => scribble;
        public TMP_Text HoverShadow => shadowEcho;
        // Legacy read-only access lets old diagnostics confirm the object is inactive.
        public ProceduralDisabledSlashGraphic DisabledSlash => disabledSlash;
        public bool AutoSizesHitboxToText => autoSizeHitboxToText;

        private MenuInteractionSettings Settings
        {
            get
            {
                if (interactionSettings == null)
                {
                    interactionSettings = ScriptableObject.CreateInstance<MenuInteractionSettings>();
                    interactionSettings.hideFlags = HideFlags.DontSave;
                }
                return interactionSettings;
            }
        }

        private void Awake()
        {
            // 这些自动查找只用于降低批量接入成本；正式 Prefab 仍建议显式绑定，避免层级改动后找错对象。
            if (button == null) button = GetComponent<Button>();
            if (label == null) label = GetComponentInChildren<TMP_Text>(true);
            if (labelVisualRoot == null && label != null) labelVisualRoot = label.rectTransform;
            if (interactionVisualRoot == null && label != null)
            {
                RectTransform labelParent = label.rectTransform.parent as RectTransform;
                interactionVisualRoot = labelParent != null && labelParent != transform ? labelParent : null;
            }
            if (interactionVisualRoot == transform)
            {
                interactionVisualRoot = null;
                Debug.LogWarning("InteractionVisualRoot must be a child of the Button so hover visuals cannot change its raycast hitbox.", this);
            }
            EnsureFixedHitTarget();

            random = new System.Random(unchecked(Environment.TickCount * 397) ^ seedSalt ^ GetInstanceID());
            labelBaseScale = labelVisualRoot != null ? labelVisualRoot.localScale : Vector3.one;
            visualBaseScale = interactionVisualRoot != null ? interactionVisualRoot.localScale : Vector3.one;
            labelVisualBasePosition = labelVisualRoot != null ? labelVisualRoot.anchoredPosition : Vector2.zero;
            visualBasePosition = interactionVisualRoot != null ? interactionVisualRoot.anchoredPosition : Vector2.zero;
            visualBaseRotation = interactionVisualRoot != null ? interactionVisualRoot.localRotation : Quaternion.identity;
            if (label != null)
            {
                labelBaseColor = label.color;
                labelBaseOutlineWidth = label.outlineWidth;
                labelBaseOutlineColor = label.outlineColor;
                labelBaseSharedMaterial = label.fontSharedMaterial;
                baseMaterialFontInstanceId = label.font != null ? label.font.GetInstanceID() : 0;
            }

            EnsureTextShadow();

            if (variant == MenuButtonVariant.Highlighted && autoCreateHighlightEchoes)
            {
                EnsureHighlightEchoes();
            }
            SyncEchoAppearance();
            ApplyEchoAndShadowPositions();

            // v1.0.11 removes the disabled strike. Existing scenes may still deserialize the
            // historical object; deactivate it instead of destroying user-authored hierarchy.
            if (disabledSlash != null)
            {
                disabledSlash.SetDisabledVisible(false);
                disabledSlash.gameObject.SetActive(false);
            }

            previousInteractable = button == null || button.interactable;
            ApplyInteractable(previousInteractable);
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
            lastLayoutSignature = CaptureLayoutSignature();
            hasLayoutSignature = label != null;
        }

        private void Start()
        {
            hasStarted = true;
            StartHighlightPromptLoopIfNeeded();
            activateAfterInitialLayout = drawOnStart && previousInteractable;
            RequestLayoutRefresh(true);
        }

        private void Update()
        {
            bool interactable = button == null || button.interactable;
            if (interactable != previousInteractable)
            {
                previousInteractable = interactable;
                ApplyInteractable(interactable);
            }
        }

        private void OnDestroy()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }

        private void OnDisable()
        {
            if (current == this) current = null;
            StopAllCoroutines();
            hoverRoutine = null;
            pressRoutine = null;
            confirmRoutine = null;
            highlightRoutine = null;
            layoutRefreshRoutine = null;
            layoutRefreshRequested = false;
            regeneratePathOnLayoutRefresh = false;
            synchronizingTextAndLayout = false;
            activateAfterInitialLayout = false;
            pointerInside = false;
            focused = false;
            selectedByPointer = false;
            confirming = false;
            RestoreBaseVisuals();
            scribble?.Hide(0f);
        }

        private void OnEnable()
        {
            // Start 只会执行一次；菜单面板关闭后重新打开时，需要重新恢复长期高亮提示循环。
            if (!hasStarted) return;
            previousInteractable = button == null || button.interactable;
            ApplyInteractable(previousInteractable);
            RequestLayoutRefresh(true);
            StartHighlightPromptLoopIfNeeded();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!previousInteractable) return;
            pointerInside = true;
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != gameObject)
            {
                selectedByPointer = true;
                EventSystem.current.SetSelectedGameObject(gameObject, eventData);
            }
            else
            {
                Activate();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointerInside = false;
            AnimatePress(false);
            if (selectedByPointer && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                selectedByPointer = false;
                EventSystem.current.SetSelectedGameObject(null, eventData);
                focused = false;
            }
            if (!focused && !confirming)
            {
                Deactivate();
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!previousInteractable) return;
            focused = true;
            Activate();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            focused = false;
            selectedByPointer = false;
            if (!pointerInside && !confirming)
            {
                Deactivate();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!previousInteractable || eventData.button != PointerEventData.InputButton.Left) return;
            if (current != this) Activate();
            Press();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            AnimatePress(false);
            StartCoroutine(RestorePaletteIfClickDidNotArrive());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Confirm();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (previousInteractable && !confirming)
            {
                StartCoroutine(SubmitSequence());
            }
        }

        public void SetReducedMotion(bool value)
        {
            reducedMotion = value;
        }

        /// <summary>
        /// 文字、字体、字号、本地化语言或布局在运行时变化后调用。
        /// TMP 文本变化事件也会自动刷新，但项目若同帧替换字体和布局，建议在布局完成后的下一帧显式调用一次。
        /// </summary>
        public void RefreshVisualState()
        {
            RequestLayoutRefresh(true);
        }

        public void Activate()
        {
            if (!previousInteractable || scribble == null)
            {
                return;
            }

            // 全菜单同一时间只保留一个可见 Scribble，避免多个按钮同时发光和叠加透明过绘。
            if (current != null && current != this)
            {
                current.ForceInactive();
            }

            if (current == this && scribble.HasVisiblePath)
            {
                AnimateHover(true);
                return;
            }

            current = this;
            uint seed = ((uint)random.Next(1, int.MaxValue) << 1) ^ (uint)random.Next(1, int.MaxValue);
            scribble.BeginDraw(seed, reducedMotion);
            AnimateHover(true);
            onScribbleDrawn?.Invoke();
        }

        private void EnsureFixedHitTarget()
        {
            if (button == null) return;
            // 射线只落在 Button 根节点的 Graphic。霓虹、文字、墨点、删除线都不能扩大命中区域。
            Graphic fixedTarget = GetComponent<Graphic>();
            if (fixedTarget == null)
            {
                Image transparentHitImage = gameObject.AddComponent<Image>();
                transparentHitImage.color = Color.clear;
                fixedTarget = transparentHitImage;
            }
            fixedTarget.raycastTarget = true;
            button.targetGraphic = fixedTarget;
            if (label != null && label != fixedTarget)
            {
                label.raycastTarget = false;
            }
        }

        private void RequestLayoutRefresh(bool regenerateCurrentPath)
        {
            layoutRefreshRequested = true;
            regeneratePathOnLayoutRefresh |= regenerateCurrentPath;
            if (!isActiveAndEnabled || layoutRefreshRoutine != null)
            {
                return;
            }

            layoutRefreshRoutine = StartCoroutine(DeferredLayoutRefresh());
        }

        private IEnumerator DeferredLayoutRefresh()
        {
            // TMP 的 TEXT_CHANGED_EVENT 可能在 Canvas rebuild 调用栈内触发。至少推迟一帧，
            // 并等待布局/图形重建结束后再 ForceMeshUpdate 或 SetVerticesDirty。
            yield return null;
            while (CanvasUpdateRegistry.IsRebuildingLayout() || CanvasUpdateRegistry.IsRebuildingGraphics())
            {
                yield return null;
            }

            bool regenerateCurrentPath = regeneratePathOnLayoutRefresh;
            layoutRefreshRequested = false;
            regeneratePathOnLayoutRefresh = false;
            layoutRefreshRoutine = null;
            SynchronizeTextAndLayout(regenerateCurrentPath);

            if (activateAfterInitialLayout)
            {
                activateAfterInitialLayout = false;
                if (previousInteractable)
                {
                    Activate();
                }
            }

            // 刷新期间若外部又改变了真实布局属性，继续安排下一帧；同帧重复事件仍只执行一次。
            if (layoutRefreshRequested && isActiveAndEnabled && layoutRefreshRoutine == null)
            {
                layoutRefreshRoutine = StartCoroutine(DeferredLayoutRefresh());
            }
        }

        private void SynchronizeTextAndLayout(bool regenerateCurrentPath)
        {
            if (synchronizingTextAndLayout)
            {
                return;
            }

            synchronizingTextAndLayout = true;
            try
            {
                EnsureTextShadow();
                SyncEchoAppearance();
                ApplyFixedHitboxSize();
                scribble?.RefreshLayoutImmediately(regenerateCurrentPath);
                previousInteractable = button == null || button.interactable;
                ApplyInteractable(previousInteractable);
                lastLayoutSignature = CaptureLayoutSignature();
                hasLayoutSignature = label != null;
            }
            finally
            {
                synchronizingTextAndLayout = false;
            }
        }

        private LayoutSignature CaptureLayoutSignature()
        {
            if (label == null)
            {
                return default;
            }

            return new LayoutSignature
            {
                text = label.text,
                fontInstanceId = label.font != null ? label.font.GetInstanceID() : 0,
                fontSize = label.fontSize,
                fontStyle = label.fontStyle,
                fontWeight = label.fontWeight,
                alignment = label.alignment,
                characterSpacing = label.characterSpacing,
                wordSpacing = label.wordSpacing,
                lineSpacing = label.lineSpacing,
                paragraphSpacing = label.paragraphSpacing,
                overflowMode = label.overflowMode,
                richText = label.richText,
                enableAutoSizing = label.enableAutoSizing,
                enableWordWrapping = label.enableWordWrapping,
                margin = label.margin,
                rectSize = label.rectTransform.rect.size
            };
        }

        private void ApplyFixedHitboxSize()
        {
            if (!autoSizeHitboxToText || label == null) return;
            RectTransform buttonRect = transform as RectTransform;
            if (buttonRect == null) return;

            // 空文本通常发生在业务本地化尚未赋值的初始化窗口。此时保留 Prefab 已创作的
            // Button 尺寸，不能把真实命中框缩成 1px + padding。
            if (string.IsNullOrEmpty(label.text))
            {
                if (!warnedInvalidLabelLayout)
                {
                    warnedInvalidLabelLayout = true;
                    Debug.LogWarning("Menu label is empty. The authored Button hitbox is preserved until valid text is assigned.", this);
                }
                return;
            }

            if (!HallMotionRuntimeGuards.HasUsableScale(buttonRect)
                || !HallMotionRuntimeGuards.HasUsableScale(label.rectTransform))
            {
                if (!warnedInvalidTransformScale)
                {
                    warnedInvalidTransformScale = true;
                    Debug.LogError("Menu Button or TMP hierarchy has zero/non-finite scale. Layout refresh was skipped; restore Canvas and visual roots to a valid scale (normally 1,1,1).", this);
                }
                return;
            }

            label.ForceMeshUpdate();
            // 读取常态文字的整体渲染边界，而不是悬停放大后的视觉根；这样 Hover 不会改变命中框。
            Bounds textBounds = label.textBounds;
            if (!HallMotionRuntimeGuards.IsFinite(textBounds) || textBounds.size.x <= 0.01f)
            {
                if (!warnedInvalidLabelLayout)
                {
                    warnedInvalidLabelLayout = true;
                    Debug.LogWarning("TMP produced empty or non-finite text bounds. The authored Button hitbox is preserved.", this);
                }
                return;
            }

            Vector3 textCenterBefore = label.rectTransform.TransformPoint(textBounds.center);
            float desiredWidth = Mathf.Max(1f, textBounds.size.x + Settings.hitPaddingX * 2f);
            if (!HallMotionRuntimeGuards.IsFinite(textCenterBefore) || !HallMotionRuntimeGuards.IsFinite(desiredWidth))
            {
                if (!warnedInvalidLabelLayout)
                {
                    warnedInvalidLabelLayout = true;
                    Debug.LogWarning("Menu text layout produced non-finite coordinates. The authored Button hitbox is preserved.", this);
                }
                return;
            }

            warnedInvalidLabelLayout = false;
            warnedInvalidTransformScale = false;
            LayoutElement layoutElement = GetComponent<LayoutElement>();
            if (layoutElement != null && !layoutElement.ignoreLayout)
            {
                layoutElement.minWidth = desiredWidth;
                layoutElement.preferredWidth = desiredWidth;
                layoutElement.flexibleWidth = 0f;
            }
            buttonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, desiredWidth);
            label.ForceMeshUpdate();
            Bounds boundsAfter = label.textBounds;
            if (HallMotionRuntimeGuards.IsFinite(boundsAfter))
            {
                Vector3 textCenterAfter = label.rectTransform.TransformPoint(boundsAfter.center);
                if (HallMotionRuntimeGuards.IsFinite(textCenterAfter))
                {
                    buttonRect.position += textCenterBefore - textCenterAfter;
                }
            }
        }

        private void Press()
        {
            scribble?.SetMonochrome(true);
            AnimatePress(true);
        }

        private void Confirm()
        {
            if (!previousInteractable || Time.unscaledTime - lastConfirmTime < 0.08f)
            {
                return;
            }

            lastConfirmTime = Time.unscaledTime;
            if (confirmRoutine != null) StopCoroutine(confirmRoutine);
            confirmRoutine = StartCoroutine(ConfirmSequence());
            onConfirmed?.Invoke();
        }

        private void Deactivate()
        {
            if (current == this) current = null;
            AnimateHover(false);
            scribble?.SetMonochrome(false);
            scribble?.Hide(reducedMotion ? 0f : Settings.hoverExitSeconds);
        }

        private void ForceInactive()
        {
            confirming = false;
            if (confirmRoutine != null) StopCoroutine(confirmRoutine);
            AnimatePress(false);
            AnimateHover(false);
            scribble?.SetMonochrome(false);
            scribble?.Hide(reducedMotion ? 0f : Settings.hoverExitSeconds);
        }

        private void ForceInactiveImmediately()
        {
            // Disabling a Button is a state change, not a hover-exit animation. A running hover
            // tween would otherwise write the normal label colour back after ApplyInteractable
            // dims it, and could leave stale pressed/confirm visuals behind the disabled slash.
            if (current == this) current = null;
            confirming = false;
            if (hoverRoutine != null) StopCoroutine(hoverRoutine);
            if (pressRoutine != null) StopCoroutine(pressRoutine);
            if (confirmRoutine != null) StopCoroutine(confirmRoutine);
            hoverRoutine = null;
            pressRoutine = null;
            confirmRoutine = null;
            scribble?.SetMonochrome(false);
            scribble?.Hide(0f);
            RestoreBaseVisuals();
        }

        private void AnimateHover(bool active)
        {
            if (hoverRoutine != null) StopCoroutine(hoverRoutine);
            hoverRoutine = StartCoroutine(HoverTween(active));
        }

        private IEnumerator HoverTween(bool active)
        {
            float duration = reducedMotion ? 0f : (active ? Settings.hoverEnterSeconds : Settings.hoverExitSeconds);
            Vector3 fromScale = labelVisualRoot != null ? labelVisualRoot.localScale : labelBaseScale;
            Vector3 toScale = labelBaseScale * (active ? Settings.hoverTextScale : 1f);
            Vector2 fromVisualPosition = interactionVisualRoot != null ? interactionVisualRoot.anchoredPosition : visualBasePosition;
            Vector2 toVisualPosition = visualBasePosition + (active ? new Vector2(18f, 0f) : Vector2.zero);
            Quaternion fromVisualRotation = interactionVisualRoot != null ? interactionVisualRoot.localRotation : visualBaseRotation;
            float buttonTilt = transform is RectTransform buttonRect ? NormalizeSignedAngle(buttonRect.localEulerAngles.z) : 0f;
            Quaternion hoverRotation = visualBaseRotation * Quaternion.Euler(0f, 0f, -2f - buttonTilt);
            Quaternion toVisualRotation = active ? hoverRotation : visualBaseRotation;
            float fromShadowOutline = shadowEcho != null ? shadowEcho.outlineWidth : 0f;
            float targetShadowOutline = active ? CalculateHoverShadowOutlineWidth() : 0f;
            Color fromColor = label != null ? label.color : labelBaseColor;
            Color toColor = active ? Color.white : labelBaseColor;
            float fromHoverProgress = hoverVisualProgress;
            float toHoverProgress = active ? 1f : 0f;
            float elapsed = 0f;
            do
            {
                float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                ApplyLabelScale(Vector3.LerpUnclamped(fromScale, toScale, eased));
                hoverVisualProgress = Mathf.LerpUnclamped(fromHoverProgress, toHoverProgress, eased);
                if (interactionVisualRoot != null)
                {
                    interactionVisualRoot.anchoredPosition = Vector2.LerpUnclamped(fromVisualPosition, toVisualPosition, eased);
                    interactionVisualRoot.localRotation = Quaternion.SlerpUnclamped(fromVisualRotation, toVisualRotation, eased);
                }
                if (label != null)
                {
                    label.color = Color.LerpUnclamped(fromColor, toColor, eased);
                }
                if (shadowEcho != null)
                {
                    shadowEcho.outlineWidth = Mathf.Lerp(fromShadowOutline, targetShadowOutline, eased);
                    shadowEcho.outlineColor = Color.black;
                }
                ApplyEchoAndShadowPositions();
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            while (elapsed < duration);
            ApplyLabelScale(toScale);
            hoverVisualProgress = toHoverProgress;
            if (interactionVisualRoot != null)
            {
                interactionVisualRoot.anchoredPosition = toVisualPosition;
                interactionVisualRoot.localRotation = toVisualRotation;
            }
            if (label != null)
            {
                label.color = toColor;
            }
            if (shadowEcho != null)
            {
                shadowEcho.outlineWidth = targetShadowOutline;
                shadowEcho.outlineColor = Color.black;
            }
            ApplyEchoAndShadowPositions();
            hoverRoutine = null;
        }

        private float CalculateHoverShadowOutlineWidth()
        {
            if (label == null) return 0f;
            // Parent scaling would otherwise enlarge the effect together with the label.
            // Divide by the final hover scale so the approved 3px size stays a screen-pixel
            // value at every designer-selected text scale, including the new 2x maximum.
            float finalScale = Mathf.Max(1f, Settings.hoverTextScale);
            return Mathf.Clamp01(
                HoverShadowSizePixels * WebPixelToTmpOutlineScale
                / (Mathf.Max(1f, label.fontSize) * finalScale));
        }

        private static float NormalizeSignedAngle(float degrees)
        {
            return degrees > 180f ? degrees - 360f : degrees;
        }

        private void AnimatePress(bool active)
        {
            if (interactionVisualRoot == null) return;
            if (pressRoutine != null) StopCoroutine(pressRoutine);
            pressRoutine = StartCoroutine(PressTween(active));
        }

        private IEnumerator PressTween(bool active)
        {
            float duration = reducedMotion ? 0f : (active ? 0.055f : 0.09f);
            Vector3 from = interactionVisualRoot.localScale;
            Vector3 to = visualBaseScale * (active ? Settings.pressScale : 1f);
            float elapsed = 0f;
            do
            {
                float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                interactionVisualRoot.localScale = Vector3.LerpUnclamped(from, to, eased);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            while (elapsed < duration);
            interactionVisualRoot.localScale = to;
            pressRoutine = null;
        }

        private IEnumerator ConfirmSequence()
        {
            confirming = true;
            scribble?.SetCollapseProgress(0f);
            float duration = reducedMotion ? 0f : Settings.confirmDurationSeconds;
            float elapsed = 0f;
            while (elapsed < duration || duration <= 0f)
            {
                float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                float pop = t < 0.48f
                    ? Mathf.Lerp(Settings.pressScale, 1.035f, t / 0.48f)
                    : Mathf.Lerp(1.035f, 1f, (t - 0.48f) / 0.52f);
                if (interactionVisualRoot != null)
                {
                    interactionVisualRoot.localScale = visualBaseScale * pop;
                }
                float collapse = 1f - Mathf.Pow(1f - t, 2.4f);
                scribble?.SetCollapseProgress(collapse);
                if (duration <= 0f) break;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (interactionVisualRoot != null) interactionVisualRoot.localScale = visualBaseScale;
            scribble?.ClearCollapse();
            confirming = false;
            if (!pointerInside && !focused)
            {
                Deactivate();
            }
            confirmRoutine = null;
            onConfirmVisualCompleted?.Invoke();
        }

        private IEnumerator SubmitSequence()
        {
            Activate();
            Press();
            if (!reducedMotion) yield return new WaitForSecondsRealtime(0.055f);
            Confirm();
        }

        private IEnumerator RestorePaletteIfClickDidNotArrive()
        {
            float releaseTime = Time.unscaledTime;
            yield return null;
            if (!confirming && lastConfirmTime < releaseTime)
            {
                scribble?.SetMonochrome(false);
            }
        }

        private void ApplyInteractable(bool interactable)
        {
            if (!interactable)
            {
                pointerInside = false;
                focused = false;
                selectedByPointer = false;
                ForceInactiveImmediately();
            }

            // Disabled buttons keep only the dimmed text state. The historical slash remains
            // permanently inactive even when an old Prefab still serializes the reference.
            if (disabledSlash != null)
            {
                disabledSlash.SetDisabledVisible(false);
                if (disabledSlash.gameObject.activeSelf) disabledSlash.gameObject.SetActive(false);
            }
            if (label != null)
            {
                label.color = interactable
                    ? labelBaseColor
                    : new Color(labelBaseColor.r, labelBaseColor.g, labelBaseColor.b, labelBaseColor.a * 0.28f);
            }
            if (shadowEcho != null)
            {
                shadowEcho.color = new Color(0f, 0f, 0f, interactable ? 1f : 0.28f);
            }
            SetEchoesVisible(interactable && variant == MenuButtonVariant.Highlighted);
        }

        private IEnumerator HighlightPromptLoop()
        {
            while (true)
            {
                float delay = Mathf.Lerp(Settings.highlightPromptMinSeconds, Settings.highlightPromptMaxSeconds, (float)random.NextDouble());
                yield return new WaitForSecondsRealtime(delay);
                if (!previousInteractable || pointerInside || focused || confirming || reducedMotion)
                {
                    continue;
                }

                float duration = Settings.highlightPromptDurationSeconds;
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    if (pointerInside || focused || confirming)
                    {
                        break;
                    }
                    float t = Mathf.Clamp01(elapsed / duration);
                    float pulse = Mathf.Sin(t * Mathf.PI);
                    highlightPromptProgress = pulse;
                    ApplyEchoAndShadowPositions();
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }
                highlightPromptProgress = 0f;
                ApplyEchoAndShadowPositions();
            }
        }

        private void StartHighlightPromptLoopIfNeeded()
        {
            if (variant != MenuButtonVariant.Highlighted || highlightRoutine != null || !isActiveAndEnabled)
            {
                return;
            }
            highlightRoutine = StartCoroutine(HighlightPromptLoop());
        }

        private void RestoreBaseVisuals()
        {
            if (labelVisualRoot != null) labelVisualRoot.localScale = labelBaseScale;
            if (interactionVisualRoot != null)
            {
                interactionVisualRoot.localScale = visualBaseScale;
                interactionVisualRoot.anchoredPosition = visualBasePosition;
                interactionVisualRoot.localRotation = visualBaseRotation;
            }
            hoverVisualProgress = 0f;
            highlightPromptProgress = 0f;
            if (label != null)
            {
                label.color = labelBaseColor;
                label.outlineWidth = labelBaseOutlineWidth;
                label.outlineColor = labelBaseOutlineColor;
            }
            if (shadowEcho != null)
            {
                shadowEcho.outlineWidth = 0f;
                shadowEcho.outlineColor = Color.black;
            }
            ApplyEchoAndShadowPositions();
        }

        private void RefreshBaseFontMaterial()
        {
            if (label == null) return;
            int currentFontId = label.font != null ? label.font.GetInstanceID() : 0;
            if (labelBaseSharedMaterial != null && baseMaterialFontInstanceId == currentFontId)
            {
                return;
            }
            labelBaseSharedMaterial = label.fontSharedMaterial != null
                ? label.fontSharedMaterial
                : label.font != null ? label.font.material : null;
            baseMaterialFontInstanceId = currentFontId;
        }

        private void EnsureShadowOutlineMaterial()
        {
            if (label == null || shadowEcho == null) return;
            int currentFontId = label.font != null ? label.font.GetInstanceID() : 0;
            if (shadowOutlineMaterial != null && shadowMaterialFontInstanceId == currentFontId) return;

            RefreshBaseFontMaterial();
            shadowEcho.fontSharedMaterial = labelBaseSharedMaterial;

            // The main label keeps its authored material. Only the hidden black duplicate owns
            // a private OUTLINE_ON instance, so the fixed shadow can expand without mutating
            // shared fonts or emitting label-layout change callbacks every hover frame.
            shadowOutlineMaterial = shadowEcho.fontMaterial;
            shadowMaterialFontInstanceId = currentFontId;
            if (shadowOutlineMaterial == null
                || !shadowOutlineMaterial.HasProperty(ShaderUtilities.ID_OutlineWidth)
                || !shadowOutlineMaterial.HasProperty(ShaderUtilities.ID_OutlineColor))
            {
                if (!warnedUnsupportedOutlineMaterial)
                {
                    warnedUnsupportedOutlineMaterial = true;
                    Debug.LogWarning("Menu hover shadow requires a TMP SDF material with _OutlineWidth and _OutlineColor. The label remains usable but the fixed shadow expansion is unavailable.", this);
                }
                return;
            }

            shadowOutlineMaterial.EnableKeyword(ShaderUtilities.Keyword_Outline);
            if (shadowOutlineMaterial.HasProperty(ShaderUtilities.ID_OutlineSoftness))
            {
                shadowOutlineMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, HoverShadowSoftness);
            }
            shadowEcho.outlineColor = Color.black;
            shadowEcho.outlineWidth = 0f;
            shadowEcho.UpdateMeshPadding();
        }

        private void EnsureTextShadow()
        {
            if (label == null) return;
            if (shadowEcho == null)
            {
                shadowEcho = CreateEcho("MenuTextShadow", new Color32(3, 2, 5, 255));
            }
            if (shadowEcho != null)
            {
                shadowEcho.transform.SetSiblingIndex(0);
                EnsureShadowOutlineMaterial();
            }
        }

        private void EnsureHighlightEchoes()
        {
            if (label == null) return;
            if (cyanEcho == null) cyanEcho = CreateEcho("HighlightEcho_Cyan", Settings.highlightCyan);
            if (magentaEcho == null) magentaEcho = CreateEcho("HighlightEcho_Magenta", Settings.highlightMagenta);
            SyncEchoAppearance();
            ApplyEchoAndShadowPositions();
        }

        private TMP_Text CreateEcho(string objectName, Color echoColor)
        {
            GameObject echoObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI), typeof(LayoutElement));
            echoObject.transform.SetParent(label.transform.parent, false);
            echoObject.transform.SetSiblingIndex(Mathf.Max(0, label.transform.GetSiblingIndex()));
            LayoutElement layout = echoObject.GetComponent<LayoutElement>();
            layout.ignoreLayout = true;
            TextMeshProUGUI echo = echoObject.GetComponent<TextMeshProUGUI>();
            echo.raycastTarget = false;
            echo.color = echoColor;
            return echo;
        }

        private void SyncEchoAppearance()
        {
            if (label == null) return;
            RefreshBaseFontMaterial();
            CopyLabelToEcho(shadowEcho, new Color32(3, 2, 5, 255));
            CopyLabelToEcho(cyanEcho, Settings.highlightCyan);
            CopyLabelToEcho(magentaEcho, Settings.highlightMagenta);
            EnsureShadowOutlineMaterial();
            ApplyEchoAndShadowPositions();
        }

        private void CopyLabelToEcho(TMP_Text echo, Color echoColor)
        {
            if (echo == null) return;
            echo.text = label.text;
            bool fontChanged = echo.font != label.font;
            echo.font = label.font;
            bool isHoverShadow = echo == shadowEcho;
            // Cyan/magenta echoes always use the authored material. The black hover shadow keeps
            // one private OUTLINE_ON material while the font is unchanged, preventing repeated
            // material allocation during text/layout synchronization.
            if (!isHoverShadow || fontChanged || shadowOutlineMaterial == null)
            {
                echo.fontSharedMaterial = labelBaseSharedMaterial != null ? labelBaseSharedMaterial : label.fontSharedMaterial;
                if (isHoverShadow)
                {
                    shadowOutlineMaterial = null;
                    shadowMaterialFontInstanceId = 0;
                }
            }
            echo.fontSize = label.fontSize;
            echo.fontStyle = label.fontStyle;
            echo.fontWeight = label.fontWeight;
            echo.alignment = label.alignment;
            echo.characterSpacing = label.characterSpacing;
            echo.wordSpacing = label.wordSpacing;
            echo.lineSpacing = label.lineSpacing;
            echo.paragraphSpacing = label.paragraphSpacing;
            echo.overflowMode = label.overflowMode;
            echo.richText = label.richText;
            echo.color = echoColor;
            RectTransform source = label.rectTransform;
            RectTransform target = echo.rectTransform;
            target.anchorMin = source.anchorMin;
            target.anchorMax = source.anchorMax;
            target.pivot = source.pivot;
            target.sizeDelta = source.sizeDelta;
            target.anchoredPosition = source.anchoredPosition;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }

        private void ApplyLabelScale(Vector3 scale)
        {
            if (labelVisualRoot != null) labelVisualRoot.localScale = scale;
            if (shadowEcho != null && (labelVisualRoot == null || !shadowEcho.transform.IsChildOf(labelVisualRoot)))
            {
                shadowEcho.rectTransform.localScale = scale;
            }
            if (cyanEcho != null && (labelVisualRoot == null || !cyanEcho.transform.IsChildOf(labelVisualRoot)))
            {
                cyanEcho.rectTransform.localScale = scale;
            }
            if (magentaEcho != null && (labelVisualRoot == null || !magentaEcho.transform.IsChildOf(labelVisualRoot)))
            {
                magentaEcho.rectTransform.localScale = scale;
            }
        }

        private void ApplyEchoAndShadowPositions()
        {
            if (label == null) return;
            Vector2 basePosition = labelVisualRoot == label.rectTransform
                ? labelVisualBasePosition
                : label.rectTransform.anchoredPosition;
            Vector2 idleCyan = new Vector2(-1f, 1f);
            Vector2 hoverCyan = new Vector2(-2f, 1f);
            Vector2 promptCyan = new Vector2(-Settings.highlightPromptOffset, 1f);
            Vector2 idleMagenta = new Vector2(2f, -2f);
            Vector2 hoverMagenta = new Vector2(3f, -2f);
            Vector2 promptMagenta = new Vector2(Settings.highlightPromptOffset, -1f);
            Vector2 cyanOffset = Vector2.LerpUnclamped(idleCyan, hoverCyan, hoverVisualProgress);
            Vector2 magentaOffset = Vector2.LerpUnclamped(idleMagenta, hoverMagenta, hoverVisualProgress);
            cyanOffset = Vector2.LerpUnclamped(cyanOffset, promptCyan, highlightPromptProgress);
            magentaOffset = Vector2.LerpUnclamped(magentaOffset, promptMagenta, highlightPromptProgress);
            if (cyanEcho != null) cyanEcho.rectTransform.anchoredPosition = basePosition + cyanOffset;
            if (magentaEcho != null) magentaEcho.rectTransform.anchoredPosition = basePosition + magentaOffset;

            Vector2 idleShadow = variant == MenuButtonVariant.Highlighted
                ? new Vector2(3f, -4f)
                : new Vector2(2f, -3f);
            float shadowRadians = HoverShadowAngleDegrees * Mathf.Deg2Rad;
            // 蓝湖的投影角度以“左”为 0°，与常规数学/CSS 的“右”为 0° 相差
            // 180°。因此 -3° 必须落在文字左侧并略微向下；Unity UI 本地 Y 轴
            // 向上，所以屏幕空间向下最终对应负的本地 Y。
            Vector2 fixedScreenOffset = new Vector2(
                -Mathf.Cos(shadowRadians) * HoverShadowDistancePixels,
                Mathf.Sin(shadowRadians) * HoverShadowDistancePixels);
            float currentHoverScale = Mathf.LerpUnclamped(1f, Mathf.Max(1f, Settings.hoverTextScale), hoverVisualProgress);
            bool offsetIsScaledByParent = labelVisualRoot != null && shadowEcho != null
                && shadowEcho.transform.IsChildOf(labelVisualRoot);
            Vector2 fixedLocalOffset = fixedScreenOffset / (offsetIsScaledByParent ? currentHoverScale : 1f);
            Vector2 shadowOffset = Vector2.LerpUnclamped(idleShadow, fixedLocalOffset, hoverVisualProgress);
            shadowOffset = Vector2.LerpUnclamped(shadowOffset, new Vector2(4f, -5f), highlightPromptProgress);
            if (shadowEcho != null) shadowEcho.rectTransform.anchoredPosition = basePosition + shadowOffset;
            if (labelVisualRoot != null)
            {
                labelVisualRoot.anchoredPosition = labelVisualBasePosition + new Vector2(highlightPromptProgress, 0f);
            }
        }

        private void SetEchoesVisible(bool value)
        {
            if (cyanEcho != null) cyanEcho.gameObject.SetActive(value);
            if (magentaEcho != null) magentaEcho.gameObject.SetActive(value);
        }

        private void OnTextChanged(UnityEngine.Object changed)
        {
            if (changed != label || synchronizingTextAndLayout)
            {
                return;
            }

            // TMP 会把颜色、材质和描边变化也上报为 TEXT_CHANGED_EVENT。悬停描边每帧
            // 变化时绝不能重算 Button 命中框；只有真实布局签名变化才请求下一帧刷新。
            LayoutSignature signature = CaptureLayoutSignature();
            if (!hasLayoutSignature || !signature.Equals(lastLayoutSignature))
            {
                RequestLayoutRefresh(true);
            }
        }
    }
}
