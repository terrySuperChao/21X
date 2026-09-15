using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Miscalculation.Motion.LevelSelect
{
    [DisallowMultipleComponent]
    public sealed class LevelCardDropTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform snapAnchor;
        public RectTransform feedbackVisual;
        public bool IsHovered { get; private set; }
        public bool IsConfirmPulsing => confirmPulseRoutine != null;
        Coroutine confirmPulseRoutine;
        RectTransform publicVisual, handVisual;
        Vector3 publicScale, handScale, publicPosition, handPosition;
        Vector3 publicPivot, handPivot;
        bool captured;

        public void OnPointerEnter(PointerEventData _) { IsHovered = true; }
        public void OnPointerExit(PointerEventData _) { IsHovered = false; }
        public bool Contains(Vector2 screen, Camera camera) => RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, screen, camera);

        // Compatibility entry point for callers that deliberately animate a single visual.
        public bool PlayConfirmPulse(float strength, float durationMs) => PlayConfirmPulse(null, strength, durationMs);

        /// <summary>
        /// At settled attachment, animate both card visuals around one shared world-space center,
        /// converted into each visual's parent coordinates. No reparenting or hierarchy allocation.
        /// Both visuals must be independent (neither may contain the other). A caller that starts
        /// another motion must cancel this feedback first so captured transforms cannot overwrite it.
        /// Legacy validPulseScale remains 1..1.15: 1 disables; excess above 1 controls impact depth.
        /// </summary>
        public bool PlayConfirmPulse(RectTransform companion, float strength, float durationMs)
        {
            ResetFeedbackImmediate();
            if (!isActiveAndEnabled || !feedbackVisual || !feedbackVisual.gameObject.activeInHierarchy) return false;
            if (companion && (companion == feedbackVisual || companion.IsChildOf(feedbackVisual)
                || feedbackVisual.IsChildOf(companion) || !companion.gameObject.activeInHierarchy)) return false;
            if (float.IsNaN(strength) || float.IsInfinity(strength) || float.IsNaN(durationMs) || float.IsInfinity(durationMs)) return false;
            if (strength <= 1f) return true;

            publicVisual = feedbackVisual; handVisual = companion;
            publicScale = publicVisual.localScale; publicPosition = publicVisual.localPosition;
            Vector3 center = handVisual ? (publicVisual.position + handVisual.position) * .5f : publicVisual.position;
            publicPivot = publicVisual.parent ? publicVisual.parent.InverseTransformPoint(center) : center;
            if (handVisual)
            {
                handScale = handVisual.localScale; handPosition = handVisual.localPosition;
                handPivot = handVisual.parent ? handVisual.parent.InverseTransformPoint(center) : center;
            }
            captured = true;
            confirmPulseRoutine = StartCoroutine(ConfirmPulseRoutine(Mathf.Clamp(strength, 1f, 1.15f), Mathf.Clamp(durationMs, 80f, 600f)));
            return true;
        }

        public void ResetFeedbackImmediate()
        {
            if (confirmPulseRoutine != null) StopCoroutine(confirmPulseRoutine);
            confirmPulseRoutine = null;
            Restore();
        }

        void Restore()
        {
            if (!captured) return;
            if (publicVisual) { publicVisual.localScale = publicScale; publicVisual.localPosition = publicPosition; }
            if (handVisual) { handVisual.localScale = handScale; handVisual.localPosition = handPosition; }
            captured = false; publicVisual = handVisual = null;
        }

        public static float EvaluateConfirmPulseScale(float normalizedTime, float strength)
        {
            float t = Mathf.Clamp01(normalizedTime), depth = Mathf.Clamp(strength - 1f, 0f, .15f);
            // Fast downward contact, a short compression hold, one small rebound, then rest.
            if (t < .24f) return Mathf.Lerp(1f, 1f - depth, Mathf.Pow(t / .24f, 2f));
            if (t < .34f) return 1f - depth;
            if (t < .62f) return Mathf.Lerp(1f - depth, 1f + depth * .22f, Mathf.SmoothStep(0f, 1f, (t - .34f) / .28f));
            return Mathf.Lerp(1f + depth * .22f, 1f, Mathf.SmoothStep(0f, 1f, (t - .62f) / .38f));
        }

        IEnumerator ConfirmPulseRoutine(float strength, float durationMs)
        {
            float duration = durationMs * .001f, elapsed = 0f;
            bool paired = handVisual;
            while (elapsed < duration && publicVisual && publicVisual.gameObject.activeInHierarchy
                && (!paired || (handVisual && handVisual.gameObject.activeInHierarchy)))
            {
                float scale = EvaluateConfirmPulseScale(elapsed / duration, strength);
                Apply(publicVisual, publicScale, publicPosition, publicPivot, scale);
                if (handVisual) Apply(handVisual, handScale, handPosition, handPivot, scale);
                elapsed += Mathf.Clamp(Time.unscaledDeltaTime, 0f, .05f);
                yield return null;
            }
            Restore(); confirmPulseRoutine = null;
        }

        static void Apply(RectTransform visual, Vector3 scale, Vector3 position, Vector3 pivot, float multiplier)
        {
            visual.localScale = new Vector3(scale.x * multiplier, scale.y * multiplier, scale.z);
            visual.localPosition = pivot + (position - pivot) * multiplier;
        }

        void OnDisable() { IsHovered = false; ResetFeedbackImmediate(); }
    }
}
