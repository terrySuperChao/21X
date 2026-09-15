using System;
using UnityEngine;

namespace Miscalculation.Motion.Common
{
    [Serializable]
    public sealed class MemoryTransitionSettings
    {
        public float appearDurationMs = 620f;
        public float disappearDurationMs = 520f;
        public float rebuildDurationMs = 360f;
        public float fragmentScale = 1.15f;
        public float fragmentSoftness = .055f;
        public float fragmentIrregularity = .68f;
        public float fringeStrength = .24f;

        public void Validate()
        {
            Range(appearDurationMs, 80, 2000, nameof(appearDurationMs));
            Range(disappearDurationMs, 80, 2000, nameof(disappearDurationMs));
            Range(rebuildDurationMs, 80, 1200, nameof(rebuildDurationMs));
            Range(fragmentScale, .45f, 2.5f, nameof(fragmentScale));
            Range(fragmentSoftness, .008f, .24f, nameof(fragmentSoftness));
            Range(fragmentIrregularity, 0, 1, nameof(fragmentIrregularity));
            Range(fringeStrength, 0, .65f, nameof(fringeStrength));
        }

        static void Range(float v, float min, float max, string name)
        {
            if (!MotionMath.Finite(v) || v < min || v > max) throw new ArgumentException(name + " 超出范围");
        }
    }

    [Serializable]
    public sealed class ImageTransferSettings
    {
        public float durationMs = 460f;
        public float heightProjectionY = 0f;
        public float startScale = 3.2f;
        public float shadowStartScale = 1.55f;
        public float shadowStartAlpha = .03f;
        public float shadowContactAlpha = .22f;
        public float impactScaleX = 1.08f;
        public float impactScaleY = .88f;
        public float rebound = .055f;
        public float rotationDeg = -2.5f;
        public float impactHoldMs = 35f;
        public string easing = "easeInCubic";

        public void Validate()
        {
            if (!MotionMath.Finite(durationMs) || durationMs < 80 || durationMs > 1600) throw new ArgumentException("transfer.durationMs 超出范围");
            if (!MotionMath.Finite(heightProjectionY) || Mathf.Abs(heightProjectionY) > 180) throw new ArgumentException("transfer.heightProjectionY 超出范围");
            if (!MotionMath.Finite(startScale) || startScale < 1f || startScale > 5f) throw new ArgumentException("transfer.startScale 超出范围");
            if (!MotionMath.Finite(shadowStartScale) || shadowStartScale < .8f || shadowStartScale > 2.5f) throw new ArgumentException("transfer.shadowStartScale 超出范围");
            if (!MotionMath.Finite(shadowStartAlpha) || shadowStartAlpha < 0 || shadowStartAlpha > .5f) throw new ArgumentException("transfer.shadowStartAlpha 超出范围");
            if (!MotionMath.Finite(shadowContactAlpha) || shadowContactAlpha < 0 || shadowContactAlpha > .65f) throw new ArgumentException("transfer.shadowContactAlpha 超出范围");
            if (!MotionMath.Finite(impactScaleX) || impactScaleX < .75f || impactScaleX > 1.35f) throw new ArgumentException("transfer.impactScaleX 超出范围");
            if (!MotionMath.Finite(impactScaleY) || impactScaleY < .65f || impactScaleY > 1.2f) throw new ArgumentException("transfer.impactScaleY 超出范围");
            if (!MotionMath.Finite(rotationDeg) || Mathf.Abs(rotationDeg) > 20) throw new ArgumentException("transfer.rotationDeg 超出范围");
            if (!MotionMath.Finite(rebound) || rebound < 0 || rebound > .25f) throw new ArgumentException("transfer.rebound 超出范围");
            if (!MotionMath.Finite(impactHoldMs) || impactHoldMs < 0 || impactHoldMs > 180) throw new ArgumentException("transfer.impactHoldMs 超出范围");
        }
    }

    [Serializable]
    public sealed class SelectionMotionSettings
    {
        public float durationMs = 720f;
        public float strokeWidth = 3.8f;
        public float edgeFeather = 1.15f;
        public float taperLength = 18f;
        public float paddingX = 18f;
        public float paddingY = 10f;
        public string color = "#440006";

        public void Validate()
        {
            if (!MotionMath.Finite(durationMs) || durationMs < 80 || durationMs > 1800) throw new ArgumentException("selection.durationMs 超出范围");
            if (!MotionMath.Finite(strokeWidth) || strokeWidth < 1 || strokeWidth > 12) throw new ArgumentException("selection.strokeWidth 超出范围");
            if (!MotionMath.Finite(edgeFeather) || edgeFeather < .25f || edgeFeather > 4) throw new ArgumentException("selection.edgeFeather 超出范围");
            if (!MotionMath.Finite(taperLength) || taperLength < 0 || taperLength > 40) throw new ArgumentException("selection.taperLength 超出范围");
            if (!IsOpaqueRgb(color)) throw new ArgumentException("selection.color 必须是 #RRGGBB 色值");
        }

        static bool IsOpaqueRgb(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 7 || value[0] != '#') return false;
            return ColorUtility.TryParseHtmlString(value, out _);
        }
    }

    [Serializable]
    public sealed class ButtonPressSettings
    {
        public float durationMs = 180f;
        public float pressScaleX = .96f;
        public float pressScaleY = .94f;
        public float offsetY = -2f;
        public float rotationDeg = 0f;
        public float overshoot = .025f;
        public string accentColor = "#7C3AED";

        public void Validate(string name)
        {
            if (!MotionMath.Finite(durationMs) || durationMs < 60 || durationMs > 800) throw new ArgumentException(name + ".durationMs 超出范围");
            if (!MotionMath.Finite(pressScaleX) || pressScaleX < .72f || pressScaleX > 1.2f) throw new ArgumentException(name + ".pressScaleX 超出范围");
            if (!MotionMath.Finite(pressScaleY) || pressScaleY < .72f || pressScaleY > 1.2f) throw new ArgumentException(name + ".pressScaleY 超出范围");
            if (!MotionMath.Finite(offsetY) || Mathf.Abs(offsetY) > 30) throw new ArgumentException(name + ".offsetY 超出范围");
            if (!MotionMath.Finite(rotationDeg) || Mathf.Abs(rotationDeg) > 12) throw new ArgumentException(name + ".rotationDeg 超出范围");
            if (!MotionMath.Finite(overshoot) || overshoot < 0 || overshoot > .25f) throw new ArgumentException(name + ".overshoot 超出范围");
        }
    }

    [Serializable]
    public sealed class FullScreenTransitionSettings
    {
        public float entranceDurationMs = 520f;
        public float exitDurationMs = 520f;
        public float entranceBlackHoldMs = 80f;
        public float exitBlackHoldMs = 80f;
        public float reducedDurationMs = 180f;
        public string easing = "easeInOutCubic";

        public void Validate()
        {
            if (!MotionMath.Finite(entranceDurationMs) || entranceDurationMs < 80 || entranceDurationMs > 1800) throw new ArgumentException("screen.entranceDurationMs 超出范围");
            if (!MotionMath.Finite(exitDurationMs) || exitDurationMs < 80 || exitDurationMs > 1800) throw new ArgumentException("screen.exitDurationMs 超出范围");
            if (!MotionMath.Finite(entranceBlackHoldMs) || entranceBlackHoldMs < 0 || entranceBlackHoldMs > 800) throw new ArgumentException("screen.entranceBlackHoldMs 超出范围");
            if (!MotionMath.Finite(exitBlackHoldMs) || exitBlackHoldMs < 0 || exitBlackHoldMs > 800) throw new ArgumentException("screen.exitBlackHoldMs 超出范围");
            if (!MotionMath.Finite(reducedDurationMs) || reducedDurationMs < 40 || reducedDurationMs > 600) throw new ArgumentException("screen.reducedDurationMs 超出范围");
        }
    }

    [Serializable]
    public sealed class CommonMotionProfile
    {
        public const int CurrentSchema = 1;
        public const string CurrentVersion = "1.0.4";
        public int schemaVersion = CurrentSchema;
        public string motionVersion = CurrentVersion;
        public string algorithmVersion = "common-motion-v1.0.4-card-contact-and-safe-release";
        public string profileId = "common.default";
        public string profileName = "全局通用动效";
        public bool reducedMotion;
        public uint seed = 0x20260908;
        public MemoryTransitionSettings itemMemory = new MemoryTransitionSettings();
        public MemoryTransitionSettings cardArtworkMemory = new MemoryTransitionSettings { appearDurationMs = 420f, disappearDurationMs = 360f, rebuildDurationMs = 320f, fragmentScale = .92f, fragmentSoftness = .045f, fragmentIrregularity = .58f, fringeStrength = .2f };
        public ImageTransferSettings transfer = new ImageTransferSettings();
        public SelectionMotionSettings difficultySelection = new SelectionMotionSettings();
        public SelectionMotionSettings chapterSelection = new SelectionMotionSettings { color = "#7B0611" };
        public ButtonPressSettings buttonA = new ButtonPressSettings();
        public ButtonPressSettings buttonB = new ButtonPressSettings { durationMs = 230, pressScaleX = .92f, pressScaleY = .92f, offsetY = 0, overshoot = .055f, accentColor = "#28D9D5" };
        public ButtonPressSettings buttonC = new ButtonPressSettings { durationMs = 320, pressScaleX = .90f, pressScaleY = .90f, offsetY = 0, rotationDeg = -6f, overshoot = .12f, accentColor = "#22D7E8" };
        public FullScreenTransitionSettings screen = new FullScreenTransitionSettings();

        public CommonMotionProfile Clone() => JsonUtility.FromJson<CommonMotionProfile>(JsonUtility.ToJson(this));

        public void Validate()
        {
            if (schemaVersion != CurrentSchema) throw new ArgumentException("通用动效 Schema 不兼容");
            if (string.IsNullOrWhiteSpace(profileId) || profileId.Length > 96) throw new ArgumentException("profileId 无效");
            if (seed == 0) throw new ArgumentException("随机种子不能为 0");
            itemMemory = itemMemory ?? new MemoryTransitionSettings();
            cardArtworkMemory = cardArtworkMemory ?? new MemoryTransitionSettings();
            transfer = transfer ?? new ImageTransferSettings();
            difficultySelection = difficultySelection ?? new SelectionMotionSettings();
            chapterSelection = chapterSelection ?? new SelectionMotionSettings { color = "#7B0611" };
            buttonA = buttonA ?? new ButtonPressSettings();
            buttonB = buttonB ?? new ButtonPressSettings();
            buttonC = buttonC ?? new ButtonPressSettings();
            screen = screen ?? new FullScreenTransitionSettings();
            itemMemory.Validate(); cardArtworkMemory.Validate(); transfer.Validate(); difficultySelection.Validate(); chapterSelection.Validate();
            buttonA.Validate("buttonA"); buttonB.Validate("buttonB"); buttonC.Validate("buttonC"); screen.Validate();
        }
    }
}
