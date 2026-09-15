using System;
using UnityEngine;
using UnityEngine.UI;

namespace Miscalculation.Motion.Common
{
    [DisallowMultipleComponent]
    public sealed class FullScreenBlackTransition : MonoBehaviour
    {
        public Image blackOverlay;
        public event Action Completed;
        FullScreenTransitionSettings settings = new FullScreenTransitionSettings();
        float from, to, elapsed, duration, hold; bool running, reducedMotion;
        public void Configure(FullScreenTransitionSettings value, bool reducedMotion)
        {
            settings = value ?? new FullScreenTransitionSettings(); settings.Validate();
            this.reducedMotion = reducedMotion;
        }
        public void PlayEntrance() => Play(1, 0, settings.entranceDurationMs, settings.entranceBlackHoldMs);
        public void PlayExit() => Play(0, 1, settings.exitDurationMs, settings.exitBlackHoldMs);
        public void SetImmediate(bool black) { running = false; SetAlpha(black ? 1 : 0); enabled = false; }
        void Play(float a, float b, float durationMs, float holdMs) { if (!blackOverlay) return; from = a; to = b; elapsed = 0; duration = Mathf.Max(.04f, (reducedMotion ? settings.reducedDurationMs : durationMs) * .001f); hold = reducedMotion ? 0 : Mathf.Max(0, holdMs * .001f); running = true; SetAlpha(a); blackOverlay.raycastTarget = b > a; enabled = true; }
        void Update()
        {
            if (!running) { enabled = false; return; }
            elapsed += Mathf.Clamp(Time.unscaledDeltaTime, 0, .05f);
            bool entrance = to < from;
            float activeElapsed = entrance ? Mathf.Max(0, elapsed - hold) : elapsed;
            float t = Mathf.Clamp01(activeElapsed / Mathf.Max(.04f, duration));
            SetAlpha(Mathf.Lerp(from, to, MotionMath.Ease(settings.easing, t)));
            if (t < 1 || (!entrance && elapsed < duration + hold)) return; running = false; blackOverlay.raycastTarget = to > .5f; enabled = false; Completed?.Invoke();
        }
        void SetAlpha(float value) { if (!blackOverlay) return; Color c = blackOverlay.color; c.a = Mathf.Clamp01(value); blackOverlay.color = c; }
    }
}
