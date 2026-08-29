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
    /// 2. 驱动文字放大/描边、霓虹划线、黑白按下态、确认收束和禁用斜线；
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
        [Tooltip("负责按下/确认时整体缩放的独立子级视觉根节点。它应包含 Scribble、LabelVisualRoot 与 DisabledSlash，且绝不能是 Button 自己的 RectTransform。")]
        [SerializeField] private RectTransform interactionVisualRoot;
        [Tooltip("放在文字后方的 ProceduralNeonScribbleGraphic。该 Graphic 必须关闭 Raycast Target。")]
        [SerializeField] private ProceduralNeonScribbleGraphic scribble;
        [Tooltip("禁用态手绘删除线。可留空，运行时会在 InteractionVisualRoot 下自动创建。")]
        [SerializeField] private ProceduralDisabledSlashGraphic disabledSlash;

        [Header("Highlighted text echoes")]
        [Tooltip("Highlighted 类型缺少青紫文字回声时自动创建。正式 Prefab 也可手工创建后绑定下方两个字段。")]
        [SerializeField] private bool autoCreateHighlightEchoes = true;
        [Tooltip("Highlighted 常态的青色错位文字回声；可留空自动创建，不参与射线和布局。")]
        [SerializeField] private TMP_Text cyanEcho;
        [Tooltip("Highlighted 常态的紫色错位文字回声；可留空自动创建，不参与射线和布局。")]
        [SerializeField] private TMP_Text magentaEcho;

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
        private Color labelBaseColor;
        private float labelBaseOutlineWidth;
        private Color32 labelBaseOutlineColor;
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
            if (label != null)
            {
                labelBaseColor = label.color;
                labelBaseOutlineWidth = label.outlineWidth;
                labelBaseOutlineColor = label.outlineColor;
            }

            if (variant == MenuButtonVariant.Highlighted && autoCreateHighlightEchoes)
            {
                EnsureHighlightEchoes();
            }

            if (disabledSlash == null)
            {
                disabledSlash = CreateDisabledSlash();
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
                SyncEchoAppearance();
                ApplyFixedHitboxSize();
                scribble?.RefreshLayoutImmediately(regenerateCurrentPath);
                disabledSlash?.RefreshLayoutImmediately();
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
            float fromOutline = label != null ? label.outlineWidth : 0f;
            float targetOutline = active && label != null
                ? Mathf.Clamp01(Settings.hoverOutlineWidthPx / Mathf.Max(1f, label.fontSize))
                : labelBaseOutlineWidth;
            Color32 fromOutlineColor = label != null ? label.outlineColor : labelBaseOutlineColor;
            Color32 toOutlineColor = active ? Settings.hoverOutlineColor : labelBaseOutlineColor;
            float elapsed = 0f;
            do
            {
                float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                ApplyLabelScale(Vector3.LerpUnclamped(fromScale, toScale, eased));
                if (label != null)
                {
                    label.outlineWidth = Mathf.Lerp(fromOutline, targetOutline, eased);
                    label.outlineColor = Color32.Lerp(fromOutlineColor, toOutlineColor, eased);
                }
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            while (elapsed < duration);
            ApplyLabelScale(toScale);
            if (label != null)
            {
                label.outlineWidth = targetOutline;
                label.outlineColor = toOutlineColor;
            }
            hoverRoutine = null;
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
            disabledSlash?.SetDisabledVisible(!interactable);
            if (label != null)
            {
                label.color = interactable
                    ? labelBaseColor
                    : new Color(labelBaseColor.r, labelBaseColor.g, labelBaseColor.b, labelBaseColor.a * 0.28f);
            }
            SetEchoesVisible(interactable && variant == MenuButtonVariant.Highlighted);
            if (!interactable)
            {
                pointerInside = false;
                focused = false;
                ForceInactive();
            }
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
                    ApplyEchoOffsets(Mathf.Lerp(1f, Settings.highlightPromptOffset, pulse));
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }
                ApplyEchoOffsets(1f);
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
            if (interactionVisualRoot != null) interactionVisualRoot.localScale = visualBaseScale;
            if (label != null)
            {
                label.color = labelBaseColor;
                label.outlineWidth = labelBaseOutlineWidth;
                label.outlineColor = labelBaseOutlineColor;
            }
        }

        private void EnsureHighlightEchoes()
        {
            if (label == null) return;
            if (cyanEcho == null) cyanEcho = CreateEcho("HighlightEcho_Cyan", Settings.highlightCyan);
            if (magentaEcho == null) magentaEcho = CreateEcho("HighlightEcho_Magenta", Settings.highlightMagenta);
            SyncEchoAppearance();
            ApplyEchoOffsets(1f);
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

        private ProceduralDisabledSlashGraphic CreateDisabledSlash()
        {
            RectTransform parent = interactionVisualRoot != null ? interactionVisualRoot : transform as RectTransform;
            if (parent == null) return null;
            GameObject slashObject = new GameObject("DisabledSlash", typeof(RectTransform), typeof(CanvasRenderer), typeof(ProceduralDisabledSlashGraphic), typeof(LayoutElement));
            slashObject.transform.SetParent(parent, false);
            slashObject.transform.SetAsLastSibling();
            LayoutElement layout = slashObject.GetComponent<LayoutElement>();
            layout.ignoreLayout = true;
            ProceduralDisabledSlashGraphic graphic = slashObject.GetComponent<ProceduralDisabledSlashGraphic>();
            graphic.BindText(label);
            return graphic;
        }

        private void SyncEchoAppearance()
        {
            if (label == null) return;
            CopyLabelToEcho(cyanEcho, Settings.highlightCyan);
            CopyLabelToEcho(magentaEcho, Settings.highlightMagenta);
        }

        private void CopyLabelToEcho(TMP_Text echo, Color echoColor)
        {
            if (echo == null) return;
            echo.text = label.text;
            echo.font = label.font;
            echo.fontSharedMaterial = label.fontSharedMaterial;
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
            if (cyanEcho != null && (labelVisualRoot == null || !cyanEcho.transform.IsChildOf(labelVisualRoot)))
            {
                cyanEcho.rectTransform.localScale = scale;
            }
            if (magentaEcho != null && (labelVisualRoot == null || !magentaEcho.transform.IsChildOf(labelVisualRoot)))
            {
                magentaEcho.rectTransform.localScale = scale;
            }
        }

        private void ApplyEchoOffsets(float offset)
        {
            if (label == null) return;
            Vector2 basePosition = label.rectTransform.anchoredPosition;
            if (cyanEcho != null) cyanEcho.rectTransform.anchoredPosition = basePosition + new Vector2(-offset, -offset * 0.35f);
            if (magentaEcho != null) magentaEcho.rectTransform.anchoredPosition = basePosition + new Vector2(offset, offset * 0.35f);
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
