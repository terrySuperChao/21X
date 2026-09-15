using System;
using UnityEngine;

namespace Miscalculation.Motion.LevelSelect
{
    [Serializable]
    public sealed class LevelSelectMotionProfile
    {
        public int schemaVersion = 1;
        public string motionVersion = "1.0.4";
        public string profileId = "level-select.default";
        public float dragScale = 1.08f;
        public float dragTiltMaxDeg = 5f;
        public float dragTiltFullSpeedPxPerSec = 650f;
        public float snapDurationMs = 220f;
        public float returnDurationMs = 280f;
        public float returnArcPx = 42f;
        public float validPulseScale = 1.035f;
        public float validPulseDurationMs = 180f;
        public float rebuildDelayMs = 45f;
        public void Validate()
        {
            dragScale = Mathf.Clamp(dragScale, 1, 1.3f); dragTiltMaxDeg = Mathf.Clamp(dragTiltMaxDeg, 0, 15);
            dragTiltFullSpeedPxPerSec = Mathf.Clamp(dragTiltFullSpeedPxPerSec, 120, 2400);
            snapDurationMs = Mathf.Clamp(snapDurationMs, 80, 800); returnDurationMs = Mathf.Clamp(returnDurationMs, 80, 1000);
            returnArcPx = Mathf.Clamp(returnArcPx, 0, 260); validPulseScale = Mathf.Clamp(validPulseScale, 1, 1.15f);
            validPulseDurationMs = Mathf.Clamp(validPulseDurationMs, 80, 600); rebuildDelayMs = Mathf.Clamp(rebuildDelayMs, 0, 400);
        }
    }
}
