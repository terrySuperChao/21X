using UnityEngine;
using UnityEngine.UI;

namespace Miscalculation.Motion.Common
{
    [DisallowMultipleComponent]
    public sealed class ButtonPressMotion : MonoBehaviour
    {
        public enum Style { A, B, C }
        public RectTransform visualRoot;
        public Graphic accentGraphic;
        [Tooltip("图标按钮可选的无射线回声层；Style C 会让它扩散并淡出。")]
        public Graphic pulseGraphic;
        public Button targetButton;
        public bool bindToClick;
        public Style boundStyle;
        CommonMotionProfile profile = new CommonMotionProfile();
        ButtonPressSettings active;
        Style activeStyle;
        Vector2 basePosition; Vector3 baseScale; Quaternion baseRotation; Color baseAccent, basePulse;
        RectTransform capturedVisual;
        float elapsed; bool running, bound;

        void Awake()
        {
            if (!visualRoot) visualRoot = transform as RectTransform;
            if (!targetButton) targetButton = GetComponentInParent<Button>();
            CaptureBase(true);
        }
        void Start() { CaptureBase(true); }
        void OnEnable() { if (bindToClick && targetButton && !bound) { targetButton.onClick.AddListener(PlayBound); bound = true; } }
        void OnDisable() { if (bound && targetButton) targetButton.onClick.RemoveListener(PlayBound); bound = false; Restore(); }
        public void Configure(CommonMotionProfile value) { profile = value ?? new CommonMotionProfile(); profile.Validate(); if (!running) CaptureBase(true); }
        public void PlayA() => Play(Style.A); public void PlayB() => Play(Style.B); public void PlayC() => Play(Style.C); public void PlayBound() => Play(boundStyle);
        public void Play(Style style)
        {
            if (running) Restore();
            if (capturedVisual != visualRoot) CaptureBase(true);
            activeStyle = style; active = style == Style.A ? profile.buttonA : style == Style.B ? profile.buttonB : profile.buttonC;
            elapsed = 0; running = true; enabled = true;
        }
        void Update()
        {
            if (!running || active == null) return;
            elapsed += Mathf.Clamp(Time.unscaledDeltaTime, 0, .05f); float t = Mathf.Clamp01(elapsed / Mathf.Max(.06f, active.durationMs * .001f));
            float pulse = Mathf.Sin(t * Mathf.PI);
            if (activeStyle == Style.C)
            {
                float compression = t < .28f ? Mathf.SmoothStep(0, 1, t / .28f) : Mathf.SmoothStep(1, 0, (t - .28f) / .72f);
                float pop = t > .22f ? Mathf.Sin(Mathf.Clamp01((t - .22f) / .78f) * Mathf.PI) * active.overshoot : 0;
                visualRoot.anchoredPosition = basePosition;
                visualRoot.localScale = Vector3.Scale(baseScale, new Vector3(Mathf.Lerp(1, active.pressScaleX, compression) + pop, Mathf.Lerp(1, active.pressScaleY, compression) + pop, 1));
                visualRoot.localRotation = baseRotation * Quaternion.Euler(0, 0, active.rotationDeg * Mathf.Sin(t * Mathf.PI * 2f) * (1f - t));
                if (pulseGraphic)
                {
                    pulseGraphic.rectTransform.localScale = Vector3.one * Mathf.Lerp(.78f, 1.42f, MotionMath.Ease("easeOutCubic", t));
                    Color echo = basePulse; echo.a = Mathf.Sin(t * Mathf.PI) * .42f; pulseGraphic.color = echo;
                }
            }
            else
            {
                float rebound = t > .55f ? Mathf.Sin((t - .55f) / .45f * Mathf.PI) * active.overshoot : 0;
                visualRoot.anchoredPosition = basePosition + Vector2.up * active.offsetY * pulse;
                visualRoot.localScale = Vector3.Scale(baseScale, new Vector3(Mathf.Lerp(1, active.pressScaleX, pulse) + rebound, Mathf.Lerp(1, active.pressScaleY, pulse) + rebound, 1));
                visualRoot.localRotation = baseRotation * Quaternion.Euler(0, 0, active.rotationDeg * pulse);
            }
            if (accentGraphic && ColorUtility.TryParseHtmlString(active.accentColor, out Color accent)) accentGraphic.color = Color.Lerp(baseAccent, accent, pulse * .6f);
            if (t < 1) return;
            running = false; Restore();
            // 绑定业务 Button 时必须保持组件启用，否则 OnDisable 会移除点击监听，导致只能播放一次。
            if (!bindToClick) enabled = false;
        }
        void CaptureBase(bool force) { if (!visualRoot || (!force && capturedVisual == visualRoot)) return; capturedVisual = visualRoot; basePosition = visualRoot.anchoredPosition; baseScale = visualRoot.localScale; baseRotation = visualRoot.localRotation; if (accentGraphic) baseAccent = accentGraphic.color; if (pulseGraphic) basePulse = pulseGraphic.color; }
        void Restore() { if (!visualRoot || capturedVisual != visualRoot) return; visualRoot.anchoredPosition = basePosition; visualRoot.localScale = baseScale; visualRoot.localRotation = baseRotation; if (accentGraphic) accentGraphic.color = baseAccent; if (pulseGraphic) { pulseGraphic.color = basePulse; pulseGraphic.rectTransform.localScale = Vector3.one; } }
    }
}
