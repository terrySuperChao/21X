using System;
using UnityEngine;

namespace Duel21.FeelLab
{
    /// <summary>
    /// Mirrors the browser FeelProfileV1 JSON. Values use milliseconds,
    /// 1920x1080 reference pixels, degrees, and normalized ratios.
    /// </summary>
    [Serializable]
    public sealed class FeelProfileV1
    {
        public int schemaVersion = 1;
        public string name = "利落赌桌";
        public DrawSettings draw = new DrawSettings();
        public FlightSettings flight = new FlightSettings();
        public LandSettings land = new LandSettings();
        public FlipSettings flip = new FlipSettings();
        public SequenceSettings sequence = new SequenceSettings();
        public LayoutSettings layout = new LayoutSettings();
        public EffectsSettings effects = new EffectsSettings();
        public AudioSettings audio = new AudioSettings();

        [Serializable]
        public sealed class DrawSettings
        {
            public float anticipationMs = 72f;
            public float liftPx = 25f;
            public float shadowPx = 25f;
        }

        [Serializable]
        public sealed class FlightSettings
        {
            public float durationMs = 365f;
            public float arcPx = 150f;
            public float lateralPx = 24f;
            public float startRotationDeg = 11f;
            public float spinDeg = 17f;
            public float startScale = 0.96f;
            public string easing = "easeOutCubic";
        }

        [Serializable]
        public sealed class LandSettings
        {
            public float durationMs = 220f;
            public float squashX = 0.965f;
            public float squashY = 1.04f;
            public float overshoot = 1.014f;
            public float slidePx = 13f;
            public float scatterXPx = 13f;
            public float scatterYPx = 7f;
            public float rotationJitterDeg = 2.4f;
            public float angularInertiaDeg = 2.8f;
            public float impactHoldMs = 25f;
            public float tableNudgePx = 2.2f;
        }

        [Serializable]
        public sealed class FlipSettings
        {
            // mode: afterLand, duringFlight. style: centerFold, edgePeel.
            // edgePeel is intentionally forced to afterLand for physical readability.
            public string mode = "afterLand";
            public string style = "centerFold";
            public float durationMs = 232f;
            public float midpointHoldMs = 22f;
            public float minScaleX = 0.045f;
            public float skewDeg = 4.5f;
            public float liftPx = 11f;
            public float cornerSizePx = 22f;
            public float flightStart = 0.59f;
            public string easing = "easeInOutSine";
        }

        [Serializable]
        public sealed class SequenceSettings
        {
            public float dealGapMs = 112f;
            public float revealGapMs = 190f;
            public float npcThinkMs = 355f;
            public float inputUnlockMs = 42f;
        }

        [Serializable]
        public sealed class LayoutSettings
        {
            public float handSpacingPx = 166f;
            public float reflowStartSpacingPx = 56f;
            public float fanAngleDeg = 0.75f;
            public float reflowMs = 190f;
            public float maxSpanPx = 850f;
        }

        [Serializable]
        public sealed class EffectsSettings
        {
            public float dustCount = 7f;
            [Range(0f, 1f)] public float dustVariation = 0.65f;
            public float dustSpreadPx = 46f;
            public float dustLiftPx = 18f;
            public float dustOpacity = 0.38f;
            public float dustDurationMs = 420f;
        }

        [Serializable]
        public sealed class AudioSettings
        {
            public float master = 0.72f;
            public float swoosh = 0.58f;
            public float land = 0.72f;
            public float flip = 0.62f;
            public float pitch = 1f;
            public float variation = 0.08f;
            public float timingOffsetMs = 0f;
        }

        public void ClampInPlace()
        {
            schemaVersion = 1;
            draw.anticipationMs = Mathf.Clamp(draw.anticipationMs, 0f, 450f);
            draw.liftPx = Mathf.Clamp(draw.liftPx, 0f, 90f);
            draw.shadowPx = Mathf.Clamp(draw.shadowPx, 4f, 70f);
            flight.durationMs = Mathf.Clamp(flight.durationMs, 160f, 1200f);
            flight.arcPx = Mathf.Clamp(flight.arcPx, 0f, 360f);
            flight.lateralPx = Mathf.Clamp(flight.lateralPx, -140f, 140f);
            flight.startRotationDeg = Mathf.Clamp(flight.startRotationDeg, 0f, 35f);
            flight.spinDeg = Mathf.Clamp(flight.spinDeg, -90f, 90f);
            flight.startScale = Mathf.Clamp(flight.startScale, 0.7f, 1.15f);
            land.durationMs = Mathf.Clamp(land.durationMs, 70f, 700f);
            land.squashX = Mathf.Clamp(land.squashX, 0.72f, 1.05f);
            land.squashY = Mathf.Clamp(land.squashY, 0.95f, 1.28f);
            land.overshoot = Mathf.Clamp(land.overshoot, 1f, 1.16f);
            land.slidePx = Mathf.Clamp(land.slidePx, 0f, 55f);
            land.scatterXPx = Mathf.Clamp(land.scatterXPx, 0f, 55f);
            land.scatterYPx = Mathf.Clamp(land.scatterYPx, 0f, 35f);
            land.rotationJitterDeg = Mathf.Clamp(land.rotationJitterDeg, 0f, 12f);
            land.angularInertiaDeg = Mathf.Clamp(land.angularInertiaDeg, 0f, 16f);
            land.impactHoldMs = Mathf.Clamp(land.impactHoldMs, 0f, 180f);
            land.tableNudgePx = Mathf.Clamp(land.tableNudgePx, 0f, 10f);
            flip.durationMs = Mathf.Clamp(flip.durationMs, 90f, 900f);
            flip.midpointHoldMs = Mathf.Clamp(flip.midpointHoldMs, 0f, 260f);
            flip.minScaleX = Mathf.Clamp(flip.minScaleX, 0.01f, 0.18f);
            flip.skewDeg = Mathf.Clamp(flip.skewDeg, 0f, 20f);
            flip.liftPx = Mathf.Clamp(flip.liftPx, 0f, 60f);
            flip.cornerSizePx = Mathf.Clamp(flip.cornerSizePx, 8f, 50f);
            flip.flightStart = Mathf.Clamp(flip.flightStart, 0.25f, 0.82f);
            if (flip.mode != "duringFlight") flip.mode = "afterLand";
            if (flip.style == "cornerLeft" || flip.style == "cornerRight") flip.style = "edgePeel";
            if (flip.style != "edgePeel") flip.style = "centerFold";
            if (flip.style == "edgePeel") flip.mode = "afterLand";
            effects.dustCount = Mathf.Clamp(effects.dustCount, 0f, 18f);
            effects.dustVariation = Mathf.Clamp01(effects.dustVariation);
            effects.dustSpreadPx = Mathf.Clamp(effects.dustSpreadPx, 8f, 110f);
            effects.dustLiftPx = Mathf.Clamp(effects.dustLiftPx, 0f, 55f);
            effects.dustOpacity = Mathf.Clamp01(effects.dustOpacity);
            effects.dustDurationMs = Mathf.Clamp(effects.dustDurationMs, 120f, 1200f);
            layout.reflowStartSpacingPx = Mathf.Clamp(layout.reflowStartSpacingPx, 16f, 150f);
        }

        public static FeelProfileV1 FromJson(string json)
        {
            var value = JsonUtility.FromJson<FeelProfileV1>(json);
            if (value == null) throw new ArgumentException("Invalid FeelProfileV1 JSON", nameof(json));
            value.ClampInPlace();
            return value;
        }
    }
}
