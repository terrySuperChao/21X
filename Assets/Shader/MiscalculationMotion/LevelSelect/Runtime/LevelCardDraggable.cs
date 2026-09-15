using System;
using System.Collections;
using Miscalculation.Motion.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Miscalculation.Motion.LevelSelect
{
    [DisallowMultipleComponent]
    public sealed class LevelCardDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public RectTransform motionRoot;
        public RectTransform visualRoot;
        public RectTransform coordinateRoot;
        public LevelCardDropTarget[] targets;
        public LevelSelectMotionProfile profile = new LevelSelectMotionProfile();
        [Tooltip("上层手牌的中央图案重建组件；组合时只重建这张被拖动的牌。")]
        public ImageRebuildTransition artworkRebuild;
        public Sprite baseArtwork;
        public bool clickDetaches = true;
        public LevelCardDropTarget AttachedTarget { get; private set; }
        public event Action<LevelCardDraggable, LevelCardDropTarget> Attached;
        public event Action<LevelCardDraggable, LevelCardDropTarget> Detached;
        Vector2 home, lastPointer; bool dragging; Coroutine moveRoutine; float lastDragTime;

        void Awake() { if (!motionRoot) motionRoot = transform as RectTransform; if (!visualRoot) visualRoot = motionRoot; if (!coordinateRoot) coordinateRoot = motionRoot.parent as RectTransform; home = motionRoot.anchoredPosition; }
        public void ResetImmediate()
        {
            if (AttachedTarget) AttachedTarget.ResetFeedbackImmediate();
            if (moveRoutine != null) StopCoroutine(moveRoutine); moveRoutine = null; dragging = false; AttachedTarget = null;
            if (motionRoot) motionRoot.anchoredPosition = home;
            if (visualRoot) { visualRoot.localScale = Vector3.one; visualRoot.localRotation = Quaternion.identity; }
            SetArtworkImmediate();
        }
        public void OnBeginDrag(PointerEventData e)
        {
            if (AttachedTarget) AttachedTarget.ResetFeedbackImmediate();
            if (!motionRoot || !coordinateRoot) return; if (moveRoutine != null) StopCoroutine(moveRoutine);
            dragging = true; motionRoot.SetAsLastSibling();
            if (AttachedTarget) { LevelCardDropTarget old = AttachedTarget; AttachedTarget = null; Detached?.Invoke(this, old); }
            visualRoot.localScale = Vector3.one * profile.dragScale;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(coordinateRoot, e.position, e.pressEventCamera, out Vector2 p)) lastPointer = p;
            lastDragTime = Time.unscaledTime;
        }
        public void OnDrag(PointerEventData e)
        {
            if (!dragging) return;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(coordinateRoot, e.position, e.pressEventCamera, out Vector2 p))
            {
                float now = Time.unscaledTime, dt = Mathf.Max(1f / 240f, now - lastDragTime);
                float speedX = (p.x - lastPointer.x) / dt;
                float targetTilt = -Mathf.Clamp(speedX / Mathf.Max(120f, profile.dragTiltFullSpeedPxPerSec), -1f, 1f) * profile.dragTiltMaxDeg;
                float response = 1f - Mathf.Exp(-16f * dt);
                motionRoot.anchoredPosition = p;
                visualRoot.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(visualRoot.localEulerAngles.z, targetTilt, response));
                lastPointer = p; lastDragTime = now;
            }
        }
        void Update()
        {
            if (!dragging || !visualRoot || Time.unscaledTime - lastDragTime < .06f) return;
            float response = 1f - Mathf.Exp(-12f * Mathf.Clamp(Time.unscaledDeltaTime, 0, .05f));
            visualRoot.localEulerAngles = new Vector3(0, 0, Mathf.LerpAngle(visualRoot.localEulerAngles.z, 0, response));
        }
        public void OnEndDrag(PointerEventData e)
        {
            dragging = false; LevelCardDropTarget hit = FindTarget(e.position, e.pressEventCamera);
            if (hit && hit.snapAnchor) { AttachedTarget = hit; moveRoutine = StartCoroutine(MoveTo(Local(hit.snapAnchor), profile.snapDurationMs, 0, () => Attached?.Invoke(this, hit))); }
            else moveRoutine = StartCoroutine(MoveTo(home, profile.returnDurationMs, profile.returnArcPx, null));
        }
        public void OnPointerClick(PointerEventData e)
        {
            if (dragging || !clickDetaches || !AttachedTarget) return;
            AttachedTarget.ResetFeedbackImmediate();
            LevelCardDropTarget old = AttachedTarget; AttachedTarget = null; Detached?.Invoke(this, old);
            if (moveRoutine != null) StopCoroutine(moveRoutine); moveRoutine = StartCoroutine(MoveTo(home, profile.returnDurationMs, profile.returnArcPx, null));
        }
        void OnDisable() { if (AttachedTarget) AttachedTarget.ResetFeedbackImmediate(); }
        LevelCardDropTarget FindTarget(Vector2 screen, Camera camera)
        {
            if (targets == null) return null;
            for (int i = 0; i < targets.Length; i++) if (targets[i] && targets[i].Contains(screen, camera)) return targets[i];
            return null;
        }
        Vector2 Local(RectTransform anchor) { Vector3 p = coordinateRoot.InverseTransformPoint(anchor.position); return new Vector2(p.x, p.y); }
        public void SetArtworkImmediate() { if (artworkRebuild && baseArtwork) artworkRebuild.SetImmediate(baseArtwork); }
        public bool RebuildArtwork(Sprite from, Sprite to, uint stableSeed) => artworkRebuild && artworkRebuild.Rebuild(from ? from : baseArtwork, to, stableSeed);
        /// <summary>
        /// Release settling must use the shortest signed arc. Unity serializes a visual -5° tilt
        /// as 355°, so ordinary Vector3 Euler interpolation can produce a one-frame whip.
        /// </summary>
        public static float InterpolateReleaseTilt(float startTiltDeg, float easedProgress)
        {
            return Mathf.LerpAngle(startTiltDeg, 0f, Mathf.Clamp01(easedProgress));
        }
        IEnumerator MoveTo(Vector2 destination, float durationMs, float arc, Action done)
        {
            profile.Validate();
            Vector2 start = motionRoot.anchoredPosition;
            Vector3 startScale = visualRoot.localScale;
            float startTilt = visualRoot.localEulerAngles.z;
            float elapsed = 0, duration = durationMs * .001f;
            while (elapsed < duration)
            {
                elapsed += Mathf.Clamp(Time.unscaledDeltaTime, 0, .05f);
                float t = Mathf.Clamp01(elapsed / duration);
                float e = 1 - Mathf.Pow(1 - t, 3);
                Vector2 p = Vector2.Lerp(start, destination, e);
                p.y += Mathf.Sin(t * Mathf.PI) * arc;
                motionRoot.anchoredPosition = p;
                visualRoot.localScale = Vector3.LerpUnclamped(startScale, Vector3.one, e);
                visualRoot.localEulerAngles = new Vector3(0f, 0f, InterpolateReleaseTilt(startTilt, e));
                yield return null;
            }
            motionRoot.anchoredPosition = destination; visualRoot.localScale = Vector3.one; visualRoot.localRotation = Quaternion.identity; moveRoutine = null; done?.Invoke();
        }
    }
}
