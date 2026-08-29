using System;
using System.Collections;
using UnityEngine;

namespace Duel21.FeelLab
{
    public enum CardMotionCue
    {
        Draw,
        Swoosh,
        Land,
        FlipMidpoint,
        Complete
    }

    public struct DustBurstSample
    {
        public string seed;
        public int count;
        public int edgeOffset;
        public float spreadScale;
        public float liftScale;
        public float directionBiasDeg;
        public float durationScale;
        public float markScale;
        public float markRotationDeg;
    }

    public struct ImpactApproach
    {
        public Vector2 impact;
        public Vector2 control;
        public Vector2 tangent;
        public float travelDistance;
    }

    public struct ImpactRotationPlan
    {
        public float impactRotation;
        public float angularDirection;
        public float terminalAngularDelta;
        public float angularLandingSlope;
    }

    public struct ImpactTransitionPlan
    {
        public ImpactApproach approach;
        public float effectiveSlidePx;
        public float terminalProgressSlope;
        public float landingProgressSlope;
    }

    /// <summary>
    /// Dependency-free Unity reference port of the browser motion player.
    /// Set the card RectTransform anchors and pivot to top-left (0,1), and use
    /// a CanvasScaler reference resolution of 1920x1080 for direct pixel parity.
    /// </summary>
    public sealed class CardMotionFeel : MonoBehaviour
    {
        [SerializeField] private FeelProfileV1 profile = new FeelProfileV1();
        [SerializeField, Min(0.01f)] private float playbackSpeed = 1f;
        private int dustBurstSerial;

        public FeelProfileV1 Profile
        {
            get => profile;
            set => profile = value ?? new FeelProfileV1();
        }

        public event Action<CardMotionCue> Cue;

        public Coroutine PlayDeal(
            RectTransform card,
            Vector2 start,
            Vector2 target,
            bool playerSide,
            bool revealFace,
            Action swapToFace,
            float targetRotation = 0f,
            string landingSeed = null,
            Action<float, string> applyEdgePeel = null,
            RectTransform impactVisual = null)
        {
            return StartCoroutine(PlayDealRoutine(card, start, target, playerSide, revealFace, swapToFace, targetRotation, landingSeed, applyEdgePeel, impactVisual));
        }

        public IEnumerator PlayDealRoutine(
            RectTransform card,
            Vector2 start,
            Vector2 target,
            bool playerSide,
            bool revealFace,
            Action swapToFace,
            float targetRotation = 0f,
            string landingSeed = null,
            Action<float, string> applyEdgePeel = null,
            RectTransform impactVisual = null)
        {
            if (card == null) yield break;
            profile.ClampInPlace();
            bool faceSwapped = false;
            float sideSign = playerSide ? 1f : -1f;
            string resolvedSeed = landingSeed ?? $"{(playerSide ? "player" : "npc")}:{Mathf.RoundToInt(target.x)}:{Mathf.RoundToInt(target.y)}";
            Vector3 landingDrift = SampleLandingDrift(resolvedSeed);
            Vector2 settledTarget = target + new Vector2(
                landingDrift.x * profile.land.scatterXPx,
                -landingDrift.y * profile.land.scatterYPx);
            float settledRotation = targetRotation
                - landingDrift.z * profile.land.rotationJitterDeg;
            // Browser CSS uses downward-positive Y and clockwise-positive rotation.
            // Unity UI uses upward-positive Y and counter-clockwise-positive rotation.
            float startRotation = playerSide ? profile.flight.startRotationDeg : -profile.flight.startRotationDeg;
            card.anchoredPosition = start;
            SetTransform(card, start, startRotation, Vector2.one);
            Cue?.Invoke(CardMotionCue.Draw);

            Vector2 direction = (settledTarget - start).normalized;
            Vector2 drawEnd = start - direction * 17f + Vector2.up * profile.draw.liftPx;
            yield return Animate(profile.draw.anticipationMs, "easeInCubic", (eased, raw) =>
            {
                SetTransform(
                    card,
                    Vector2.LerpUnclamped(start, drawEnd, eased),
                    Mathf.LerpUnclamped(startRotation, startRotation + sideSign * 2.2f, eased),
                    Vector2.one * Mathf.LerpUnclamped(1f, profile.flight.startScale, eased));
            });

            Vector2 flightStart = card.anchoredPosition;
            // Unity top-left anchored UI uses downward-negative Y, opposite to browser Y.
            Vector2 controlOffset = new Vector2(
                profile.flight.lateralPx * sideSign,
                playerSide ? profile.flight.arcPx : -profile.flight.arcPx);
            ImpactTransitionPlan transition = PlanImpactTransition(
                flightStart,
                settledTarget,
                controlOffset,
                profile.land.slidePx,
                profile.flight.durationMs,
                profile.land.durationMs);
            ImpactApproach approach = transition.approach;
            float terminalProgressSlope = transition.terminalProgressSlope;
            float flightStartRotation = card.localEulerAngles.z;
            if (flightStartRotation > 180f) flightStartRotation -= 360f;
            ImpactRotationPlan rotationPlan = ComputeImpactRotation(
                flightStartRotation,
                settledRotation,
                profile.land.angularInertiaDeg,
                profile.flight.spinDeg,
                -sideSign,
                terminalProgressSlope,
                profile.flight.durationMs,
                profile.land.durationMs);
            Cue?.Invoke(CardMotionCue.Swoosh);

            yield return Animate(profile.flight.durationMs, profile.flight.easing, (eased, raw) =>
            {
                float flipScale = 1f;
                float flipLift = 0f;
                float flipSkew = 0f;
                if (revealFace && profile.flip.mode == "duringFlight")
                {
                    float durationRatio = Mathf.Min(0.48f, profile.flip.durationMs / Mathf.Max(profile.flight.durationMs, 1f));
                    float flipStart = Mathf.Min(profile.flip.flightStart, 0.92f - durationRatio);
                    float progress = Mathf.Clamp01((raw - flipStart) / durationRatio);
                    if (progress > 0f)
                    {
                        float flipEase = EvaluateEase(profile.flip.easing, progress);
                        bool useEdgePeel = profile.flip.style == "edgePeel" && applyEdgePeel != null;
                        if (useEdgePeel)
                        {
                            applyEdgePeel(flipEase, profile.flip.style);
                        }
                        else
                        {
                            flipScale = flipEase < 0.5f
                                ? Mathf.LerpUnclamped(1f, profile.flip.minScaleX, flipEase * 2f)
                                : Mathf.LerpUnclamped(profile.flip.minScaleX, 1f, (flipEase - 0.5f) * 2f);
                        }
                        flipLift = Mathf.Sin(Mathf.PI * progress) * profile.flip.liftPx;
                        flipSkew = -Mathf.Sin(Mathf.PI * 2f * progress) * profile.flip.skewDeg;
                        float swapProgress = useEdgePeel ? 1f : 0.5f;
                        if (progress >= swapProgress && !faceSwapped)
                        {
                            faceSwapped = true;
                            swapToFace?.Invoke();
                            Cue?.Invoke(CardMotionCue.FlipMidpoint);
                        }
                    }
                }

                float scale = Mathf.LerpUnclamped(profile.flight.startScale, 1.028f, eased);
                float motionProgress = MonotonicFlightProgress(
                    raw,
                    terminalProgressSlope,
                    profile.flight.easing);
                Vector2 position = InertialFlightPoint(
                    flightStart,
                    approach.impact,
                    controlOffset,
                    motionProgress);
                position.y += flipLift;
                float airSpin = -Mathf.Sin(Mathf.PI * raw) * profile.flight.spinDeg * 0.35f * sideSign;
                SetTransform(
                    card,
                    position,
                    Mathf.LerpUnclamped(flightStartRotation, rotationPlan.impactRotation, motionProgress) + airSpin + flipSkew,
                    new Vector2(scale * flipScale, scale));
            });

            Vector2 landStart = card.anchoredPosition;
            float landStartRotation = card.localEulerAngles.z;
            if (landStartRotation > 180f) landStartRotation -= 360f;
            Vector2 landStartScale = card.localScale;
            RectTransform deformationVisual = impactVisual != null && impactVisual != card
                ? impactVisual
                : null;
            Cue?.Invoke(CardMotionCue.Land);
            yield return Animate(profile.land.durationMs, "easeOutCubic", (eased, raw) =>
            {
                float landingProgress = CubicLandingProgress(raw, transition.landingProgressSlope);
                Vector2 scale;
                if (raw < 0.25f)
                {
                    float k = EvaluateEase("easeOutCubic", raw / 0.25f);
                    scale = new Vector2(
                        Mathf.LerpUnclamped(landStartScale.x, profile.land.squashX, k),
                        Mathf.LerpUnclamped(landStartScale.y, profile.land.squashY, k));
                }
                else if (raw < 0.62f)
                {
                    float k = EvaluateEase("easeOutCubic", (raw - 0.25f) / 0.37f);
                    scale = new Vector2(
                        Mathf.LerpUnclamped(profile.land.squashX, profile.land.overshoot, k),
                        Mathf.LerpUnclamped(profile.land.squashY, 2f - profile.land.overshoot, k));
                }
                else
                {
                    float k = EvaluateEase("easeOutCubic", (raw - 0.62f) / 0.38f);
                    scale = new Vector2(
                        Mathf.LerpUnclamped(profile.land.overshoot, 1f, k),
                        Mathf.LerpUnclamped(2f - profile.land.overshoot, 1f, k));
                }
                float uniformScale = Mathf.LerpUnclamped(
                    (landStartScale.x + landStartScale.y) * 0.5f,
                    1f,
                    eased);
                if (deformationVisual != null)
                {
                    deformationVisual.localScale = new Vector3(
                        scale.x / uniformScale,
                        scale.y / uniformScale,
                        1f);
                }
                SetTransform(
                    card,
                    Vector2.LerpUnclamped(landStart, settledTarget, landingProgress),
                    Mathf.LerpUnclamped(
                        landStartRotation,
                        settledRotation,
                        CubicLandingProgress(raw, rotationPlan.angularLandingSlope)),
                    Vector2.one * uniformScale);
            });
            SetTransform(card, settledTarget, settledRotation, Vector2.one);
            if (deformationVisual != null)
            {
                deformationVisual.localScale = Vector3.one;
            }

            if (revealFace && profile.flip.mode == "afterLand")
            {
                yield return WaitMilliseconds(profile.land.impactHoldMs);
                yield return Flip2D(card, swapToFace, applyEdgePeel);
            }
            else if (revealFace && !faceSwapped)
            {
                swapToFace?.Invoke();
            }
            Cue?.Invoke(CardMotionCue.Complete);
        }

        public IEnumerator Flip2D(RectTransform card, Action swapFace, Action<float, string> applyEdgePeel = null)
        {
            if (card == null) yield break;
            Vector2 basePosition = card.anchoredPosition;
            float baseRotation = card.localEulerAngles.z;
            if (baseRotation > 180f) baseRotation -= 360f;
            float halfMs = Mathf.Max(1f, (profile.flip.durationMs - profile.flip.midpointHoldMs) * 0.5f);

            if (profile.flip.style == "edgePeel" && applyEdgePeel != null)
            {
                yield return Animate(halfMs, profile.flip.easing, (eased, raw) =>
                {
                    float progress = eased * 0.5f;
                    float lift = Mathf.Sin(progress * Mathf.PI);
                    applyEdgePeel(progress, profile.flip.style);
                    SetTransform(
                        card,
                        basePosition + new Vector2(-lift * profile.flip.liftPx * 0.14f, lift * profile.flip.liftPx),
                        baseRotation + lift * profile.flip.skewDeg * 0.42f,
                        new Vector2(1f - lift * 0.045f, 1f + lift * 0.014f));
                });
                yield return WaitMilliseconds(profile.flip.midpointHoldMs);
                yield return Animate(halfMs, profile.flip.easing, (eased, raw) =>
                {
                    float progress = 0.5f + eased * 0.5f;
                    float lift = Mathf.Sin(progress * Mathf.PI);
                    applyEdgePeel(progress, profile.flip.style);
                    SetTransform(
                        card,
                        basePosition + new Vector2(-lift * profile.flip.liftPx * 0.14f, lift * profile.flip.liftPx),
                        baseRotation + lift * profile.flip.skewDeg * 0.42f,
                        new Vector2(1f - lift * 0.045f, 1f + lift * 0.014f));
                });
                applyEdgePeel(1f, profile.flip.style);
                swapFace?.Invoke();
                Cue?.Invoke(CardMotionCue.FlipMidpoint);
                SetTransform(card, basePosition, baseRotation, Vector2.one);
                yield break;
            }

            yield return Animate(halfMs, profile.flip.easing, (eased, raw) =>
            {
                Vector2 position = basePosition + Vector2.up * (Mathf.Sin(raw * Mathf.PI * 0.5f) * profile.flip.liftPx);
                SetTransform(
                    card,
                    position,
                    baseRotation - Mathf.Sin(raw * Mathf.PI) * profile.flip.skewDeg,
                    new Vector2(Mathf.LerpUnclamped(1f, profile.flip.minScaleX, eased), Mathf.LerpUnclamped(1f, 1.025f, raw)));
            });
            swapFace?.Invoke();
            Cue?.Invoke(CardMotionCue.FlipMidpoint);
            yield return WaitMilliseconds(profile.flip.midpointHoldMs);
            yield return Animate(halfMs, profile.flip.easing, (eased, raw) =>
            {
                Vector2 position = basePosition + Vector2.up * (Mathf.Cos(raw * Mathf.PI * 0.5f) * profile.flip.liftPx);
                SetTransform(
                    card,
                    position,
                    baseRotation + Mathf.Sin(raw * Mathf.PI) * profile.flip.skewDeg,
                    new Vector2(Mathf.LerpUnclamped(profile.flip.minScaleX, 1f, eased), Mathf.LerpUnclamped(1.025f, 1f, raw)));
            });
            SetTransform(card, basePosition, baseRotation, Vector2.one);
        }

        private IEnumerator Animate(float milliseconds, string easing, Action<float, float> sample)
        {
            float duration = Mathf.Max(0.0001f, milliseconds / 1000f);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float raw = Mathf.Clamp01(elapsed / duration);
                sample(EvaluateEase(easing, raw), raw);
                elapsed += Time.unscaledDeltaTime * playbackSpeed;
                yield return null;
            }
            sample(1f, 1f);
        }

        private IEnumerator WaitMilliseconds(float milliseconds)
        {
            float remaining = milliseconds / 1000f;
            while (remaining > 0f)
            {
                remaining -= Time.unscaledDeltaTime * playbackSpeed;
                yield return null;
            }
        }

        private static void SetTransform(RectTransform card, Vector2 position, float rotation, Vector2 scale)
        {
            card.anchoredPosition = position;
            card.localEulerAngles = new Vector3(0f, 0f, rotation);
            card.localScale = new Vector3(scale.x, scale.y, 1f);
        }

        public static Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            float inverse = 1f - t;
            return inverse * inverse * p0 + 2f * inverse * t * p1 + t * t * p2;
        }

        public static ImpactApproach ComputeImpactApproach(
            Vector2 from,
            Vector2 target,
            Vector2 controlOffset,
            float slidePx)
        {
            Vector2 direct = target - from;
            if (direct.sqrMagnitude < 0.000001f) direct = Vector2.right;
            direct.Normalize();
            Vector2 impact = target - direct * slidePx;
            Vector2 curveOffset = PerpendicularCurveOffset(from, impact, controlOffset);
            Vector2 control = (from + impact) * 0.5f + curveOffset;
            float travelDistance = Mathf.Max(0.000001f, Vector2.Distance(from, impact));
            return new ImpactApproach
            {
                impact = impact,
                control = control,
                tangent = direct,
                travelDistance = travelDistance,
            };
        }

        public static Vector2 InertialFlightPoint(
            Vector2 from,
            Vector2 impact,
            Vector2 controlOffset,
            float progress)
        {
            float hump = Mathf.Pow(Mathf.Sin(Mathf.PI * progress), 2f) * 0.5f;
            Vector2 curveOffset = PerpendicularCurveOffset(from, impact, controlOffset);
            return Vector2.LerpUnclamped(from, impact, progress) + curveOffset * hump;
        }

        public static Vector2 PerpendicularCurveOffset(
            Vector2 from,
            Vector2 impact,
            Vector2 controlOffset)
        {
            Vector2 tangent = impact - from;
            if (tangent.sqrMagnitude < 0.000001f) tangent = Vector2.right;
            tangent.Normalize();
            return controlOffset - tangent * Vector2.Dot(controlOffset, tangent);
        }

        public static float EasingInitialSlope(string easing)
        {
            const float epsilon = 0.001f;
            return (EvaluateEase(easing, epsilon) - EvaluateEase(easing, 0f)) / epsilon;
        }

        public static float MonotonicFlightProgress(float raw, float terminalSlope, string easing)
        {
            float t = Mathf.Clamp01(raw);
            float endSlope = Mathf.Clamp(terminalSlope, 0f, 0.95f);
            float minimumStartSlope = (3f - endSlope) * 0.5f;
            float maximumStartSlope = 3f - 2f * endSlope;
            float startSlope = Mathf.Clamp(
                EasingInitialSlope(easing),
                minimumStartSlope,
                maximumStartSlope);
            float cubic = startSlope + endSlope - 2f;
            float quadratic = 3f - 2f * startSlope - endSlope;
            return cubic * t * t * t + quadratic * t * t + startSlope * t;
        }

        public static ImpactTransitionPlan PlanImpactTransition(
            Vector2 from,
            Vector2 target,
            Vector2 controlOffset,
            float requestedSlidePx,
            float flightDurationMs,
            float landDurationMs)
        {
            float totalDistance = Mathf.Max(0.000001f, Vector2.Distance(from, target));
            float flightDuration = Mathf.Max(1f, flightDurationMs);
            float landDuration = Mathf.Max(1f, landDurationMs);
            float requestedSlide = Mathf.Clamp(requestedSlidePx, 0f, totalDistance * 0.98f);
            const float maximumFlightTerminalSlope = 0.95f;
            const float minimumLandingSlope = 1.5f;
            float ratio = maximumFlightTerminalSlope * landDuration
                / (minimumLandingSlope * flightDuration);
            float maximumPhysicalSlide = totalDistance * ratio / (1f + ratio);
            float effectiveSlidePx = Mathf.Min(requestedSlide, maximumPhysicalSlide);
            ImpactApproach approach = ComputeImpactApproach(
                from,
                target,
                controlOffset,
                effectiveSlidePx);

            if (effectiveSlidePx <= 0.000001f)
            {
                return new ImpactTransitionPlan
                {
                    approach = approach,
                    effectiveSlidePx = 0f,
                    terminalProgressSlope = 0f,
                    landingProgressSlope = 0f,
                };
            }

            const float desiredLandingSlope = 3f;
            float desiredFlightSlope = desiredLandingSlope * effectiveSlidePx * flightDuration
                / (approach.travelDistance * landDuration);
            float terminalProgressSlope = Mathf.Min(maximumFlightTerminalSlope, desiredFlightSlope);
            float landingProgressSlope = terminalProgressSlope * approach.travelDistance * landDuration
                / (effectiveSlidePx * flightDuration);
            return new ImpactTransitionPlan
            {
                approach = approach,
                effectiveSlidePx = effectiveSlidePx,
                terminalProgressSlope = terminalProgressSlope,
                landingProgressSlope = Mathf.Clamp(landingProgressSlope, minimumLandingSlope, 3f),
            };
        }

        public static float CubicLandingProgress(float raw, float initialSlope)
        {
            float t = Mathf.Clamp01(raw);
            float slope = Mathf.Clamp(initialSlope, 0f, 3f);
            return slope * t
                + (3f - 2f * slope) * t * t
                + (slope - 2f) * t * t * t;
        }

        public static ImpactRotationPlan ComputeImpactRotation(
            float fromRotation,
            float settledRotation,
            float angularInertiaDeg,
            float spinDeg,
            float spinSign,
            float terminalProgressSlope,
            float flightDurationMs,
            float landDurationMs)
        {
            float inertia = Mathf.Max(0f, angularInertiaDeg);
            float airTerminalDelta = -Mathf.PI * spinDeg * 0.35f * spinSign;
            float naturalTerminalDelta = (settledRotation - fromRotation) * terminalProgressSlope
                + airTerminalDelta;
            float angularDirection = Mathf.Sign(naturalTerminalDelta);
            if (Mathf.Approximately(angularDirection, 0f))
            {
                angularDirection = Mathf.Sign(settledRotation - fromRotation);
                if (Mathf.Approximately(angularDirection, 0f)) angularDirection = 1f;
            }
            float impactRotation = settledRotation - angularDirection * inertia;
            float actualTerminalDelta = (impactRotation - fromRotation) * terminalProgressSlope
                + airTerminalDelta;
            if (!Mathf.Approximately(actualTerminalDelta, 0f)
                && Mathf.Sign(actualTerminalDelta) != angularDirection)
            {
                angularDirection = Mathf.Sign(actualTerminalDelta);
                impactRotation = settledRotation - angularDirection * inertia;
                actualTerminalDelta = (impactRotation - fromRotation) * terminalProgressSlope
                    + airTerminalDelta;
            }
            float angularLandingSlope = inertia > 0.000001f
                ? Mathf.Clamp(
                    Mathf.Abs(actualTerminalDelta) / Mathf.Max(1f, flightDurationMs)
                    * Mathf.Max(1f, landDurationMs) / inertia,
                    0f,
                    3f)
                : 0f;
            return new ImpactRotationPlan
            {
                impactRotation = impactRotation,
                angularDirection = angularDirection,
                terminalAngularDelta = actualTerminalDelta,
                angularLandingSlope = angularLandingSlope,
            };
        }

        public static uint HashString32(string value)
        {
            unchecked
            {
                uint hash = 2166136261u;
                string text = value ?? string.Empty;
                for (int index = 0; index < text.Length; index++)
                {
                    hash ^= text[index];
                    hash *= 16777619u;
                }
                return hash;
            }
        }

        public static float SeededUnit(string seed)
        {
            return HashString32(seed) / 4294967296f;
        }

        public static Vector3 SampleLandingDrift(string seed)
        {
            return new Vector3(
                SeededUnit($"{seed}:x") * 2f - 1f,
                SeededUnit($"{seed}:y") * 2f - 1f,
                SeededUnit($"{seed}:r") * 2f - 1f);
        }

        public DustBurstSample CreateDustBurst(string cardSeed)
        {
            profile.ClampInPlace();
            dustBurstSerial += 1;
            string entropy = profile.effects.dustVariation > 0f
                ? $"{dustBurstSerial}:{Time.unscaledTime:F6}:{UnityEngine.Random.value:R}"
                : "stable";
            return SampleDustBurst(profile.effects, $"{cardSeed}:dust:{entropy}");
        }

        public static DustBurstSample SampleDustBurst(FeelProfileV1.EffectsSettings effects, string seed)
        {
            int baseCount = Mathf.Max(0, Mathf.RoundToInt(effects.dustCount));
            float variation = Mathf.Clamp01(effects.dustVariation);
            int countRadius = baseCount > 0 && variation > 0f
                ? Mathf.Max(1, Mathf.RoundToInt(baseCount * 0.38f * variation))
                : 0;
            int countOffset = countRadius > 0
                ? Mathf.RoundToInt((SeededUnit($"{seed}:count") * 2f - 1f) * countRadius)
                : 0;
            return new DustBurstSample
            {
                seed = seed,
                count = Mathf.Max(0, baseCount + countOffset),
                edgeOffset = Mathf.FloorToInt(SeededUnit($"{seed}:edge") * 4f),
                spreadScale = 1f + (SeededUnit($"{seed}:spread") * 2f - 1f) * 0.46f * variation,
                liftScale = 1f + (SeededUnit($"{seed}:lift") * 2f - 1f) * 0.36f * variation,
                directionBiasDeg = (SeededUnit($"{seed}:direction") * 2f - 1f) * 18f * variation,
                durationScale = 1f + (SeededUnit($"{seed}:duration") * 2f - 1f) * 0.16f * variation,
                markScale = 1f + (SeededUnit($"{seed}:mark-scale") * 2f - 1f) * 0.1f * variation,
                markRotationDeg = (SeededUnit($"{seed}:mark-rotation") * 2f - 1f) * 1.2f * variation,
            };
        }

        public static float EvaluateEase(string easing, float t)
        {
            t = Mathf.Clamp01(t);
            switch (easing)
            {
                case "easeInCubic": return t * t * t;
                case "easeOutCubic": return 1f - Mathf.Pow(1f - t, 3f);
                case "easeInOutCubic": return t < 0.5f
                    ? 4f * t * t * t
                    : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
                case "easeInOutSine": return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
                case "easeOutBack":
                    const float c1 = 1.70158f;
                    const float c3 = c1 + 1f;
                    return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
                default: return t;
            }
        }
    }
}
