using System;
using UnityEngine;
using UnityEngine.UI;

namespace Miscalculation.Motion.Common
{
    [DisallowMultipleComponent]
    public sealed class ImageTransferMotion : MonoBehaviour
    {
        public RectTransform visual;
        public RectTransform coordinateRoot;
        [Tooltip("可选的桌面接触阴影。应与落点位于同一坐标根，且关闭射线。")]
        public Graphic contactShadow;
        public Vector2 contactShadowOffset = new Vector2(0, -8f);
        public event Action Completed;
        ImageTransferSettings settings = new ImageTransferSettings();
        Vector2 to; float elapsed; bool running;
        Color shadowTint;
        bool shadowTintCaptured;

        public void Configure(ImageTransferSettings value) { settings = value ?? new ImageTransferSettings(); settings.Validate(); }
        public bool Play(RectTransform startAnchor, RectTransform endAnchor)
        {
            return Play(endAnchor);
        }
        /// <summary>模拟垂直于桌面的高空落下：主体基本锁定落点，只通过近大远小和阴影收紧表达高度。</summary>
        public bool Play(RectTransform targetAnchor)
        {
            if (!visual || !coordinateRoot || !targetAnchor) return false;
            to = MotionMath.WorldPivotIn(targetAnchor, coordinateRoot); elapsed = 0; running = true; enabled = true;
            visual.gameObject.SetActive(true); visual.anchoredPosition = to + Vector2.up * settings.heightProjectionY;
            visual.localScale = Vector3.one * settings.startScale;
            visual.localRotation = Quaternion.Euler(0, 0, settings.rotationDeg);
            PrepareShadow(); SetShadow(settings.shadowStartScale, settings.shadowStartAlpha);
            return true;
        }
        public bool SetImmediate(RectTransform targetAnchor)
        {
            if (!visual || !coordinateRoot || !targetAnchor) return false;
            running = false; to = MotionMath.WorldPivotIn(targetAnchor, coordinateRoot); visual.gameObject.SetActive(true);
            visual.anchoredPosition = to; visual.localScale = Vector3.one; visual.localRotation = Quaternion.identity;
            PrepareShadow(); SetShadow(1f, settings.shadowContactAlpha); return true;
        }
        void Update()
        {
            if (!running) { enabled = false; return; }
            float dt = Mathf.Clamp(Time.unscaledDeltaTime, 0, .05f); elapsed += dt;
            float t = Mathf.Clamp01(elapsed / Mathf.Max(.08f, settings.durationMs * .001f));
            const float contact = .72f;
            float hold = Mathf.Clamp01(settings.impactHoldMs / Mathf.Max(1, settings.durationMs));
            if (t < contact)
            {
                float fall = Mathf.Clamp01(t / contact);
                float e = MotionMath.Ease(settings.easing, fall);
                visual.anchoredPosition = Vector2.LerpUnclamped(to + Vector2.up * settings.heightProjectionY, to, e);
                visual.localScale = Vector3.one * Mathf.Lerp(settings.startScale, 1, e);
                visual.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(settings.rotationDeg, 0, e));
                SetShadow(Mathf.Lerp(settings.shadowStartScale, .96f, e), Mathf.Lerp(settings.shadowStartAlpha, settings.shadowContactAlpha, e));
            }
            else
            {
                visual.anchoredPosition = to;
                float settle = Mathf.Clamp01((t - contact - hold) / Mathf.Max(.001f, 1 - contact - hold));
                Vector2 impact = new Vector2(settings.impactScaleX, settings.impactScaleY);
                Vector2 scale = settle <= 0 ? impact : Vector2.LerpUnclamped(impact, Vector2.one, MotionMath.Ease("easeOutBack", settle));
                float rebound = Mathf.Sin(settle * Mathf.PI) * settings.rebound;
                visual.localScale = new Vector3(scale.x + rebound, scale.y + rebound, 1);
                visual.localRotation = Quaternion.identity;
                SetShadow(Mathf.Lerp(.96f, 1f, settle), settings.shadowContactAlpha);
            }
            if (t < 1) return;
            visual.anchoredPosition = to; visual.localScale = Vector3.one; visual.localRotation = Quaternion.identity;
            SetShadow(1f, settings.shadowContactAlpha);
            running = false; enabled = false; Completed?.Invoke();
        }
        void PrepareShadow()
        {
            if (!contactShadow) return;
            if (!shadowTintCaptured) { shadowTint = contactShadow.color; shadowTint.a = 1; shadowTintCaptured = true; }
            contactShadow.gameObject.SetActive(true); contactShadow.raycastTarget = false;
            if (contactShadow.rectTransform.parent == coordinateRoot) contactShadow.rectTransform.anchoredPosition = to + contactShadowOffset;
        }
        void SetShadow(float scale, float alpha)
        {
            if (!contactShadow) return;
            contactShadow.rectTransform.localScale = Vector3.one * Mathf.Max(.01f, scale);
            Color tint = shadowTintCaptured ? shadowTint : Color.black; tint.a = Mathf.Clamp01(alpha); contactShadow.color = tint;
        }
    }
}
