using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Miscalculation.Motion.Common
{
    [DisallowMultipleComponent]
    public sealed class MemoryImageEffect : MonoBehaviour
    {
        public const int MaxSimultaneousAnimations = 24;
        public Graphic target;
        public Material materialTemplate;
        public Texture2D noiseTexture;
        public uint seed = 1;
        public bool startVisible = true;
        public UnityEvent appeared = new UnityEvent();
        public UnityEvent disappeared = new UnityEvent();

        public float Visibility { get; private set; }
        public bool IsAnimating { get; private set; }
        public static int ActiveAnimationCount => activeCount;
        public event Action<MemoryImageEffect, bool> Completed;

        Material runtimeMaterial;
        MemoryTransitionSettings settings = new MemoryTransitionSettings();
        float from, to, elapsed, duration;
        bool counted;
        static int activeCount;
        static readonly int VisibilityId = Shader.PropertyToID("_Visibility");
        static readonly int NoiseId = Shader.PropertyToID("_NoiseTex");
        static readonly int SeedId = Shader.PropertyToID("_SeedOffset");
        static readonly int ScaleId = Shader.PropertyToID("_FragmentScale");
        static readonly int SoftId = Shader.PropertyToID("_FragmentSoftness");
        static readonly int IrregularityId = Shader.PropertyToID("_FragmentIrregularity");
        static readonly int FringeId = Shader.PropertyToID("_FringeStrength");

        void Awake() { if (!target) target = GetComponent<Graphic>(); }
        void Start() { Initialize(); SetVisibleImmediate(startVisible); }
        void OnDestroy() { ReleaseSlot(); if (runtimeMaterial) Destroy(runtimeMaterial); }
        void OnDisable() { ReleaseSlot(); IsAnimating = false; }

        public void Initialize()
        {
            if (!target) target = GetComponent<Graphic>();
            if (!target || runtimeMaterial) return;
            Material source = materialTemplate;
            if (!source)
            {
                Shader shader = Shader.Find("Miscalculation/Motion/MemoryReveal");
                if (shader) source = new Material(shader);
            }
            if (!source) { Debug.LogError("[MemoryImageEffect] Missing shader/material.", this); return; }
            runtimeMaterial = new Material(source) { name = source.name + " (" + name + ")" };
            if (!materialTemplate) Destroy(source);
            target.material = runtimeMaterial;
            ApplySettings(settings);
        }

        public void ApplySettings(MemoryTransitionSettings value)
        {
            settings = value ?? new MemoryTransitionSettings(); settings.Validate(); Initialize();
            if (!runtimeMaterial) return;
            if (noiseTexture) runtimeMaterial.SetTexture(NoiseId, noiseTexture);
            runtimeMaterial.SetVector(SeedId, SeedVector(seed));
            runtimeMaterial.SetFloat(ScaleId, settings.fragmentScale);
            runtimeMaterial.SetFloat(SoftId, settings.fragmentSoftness);
            runtimeMaterial.SetFloat(IrregularityId, settings.fragmentIrregularity);
            runtimeMaterial.SetFloat(FringeId, settings.fringeStrength);
        }

        public void SetVisibleImmediate(bool visible)
        {
            ReleaseSlot(); IsAnimating = false; Visibility = visible ? 1 : 0;
            if (target) target.enabled = visible; ApplyVisibility(); enabled = false;
        }

        public bool PlayAppear() => PlayTo(true, settings.appearDurationMs * .001f);
        public bool PlayDisappear() => PlayTo(false, settings.disappearDurationMs * .001f);
        public bool PlayTo(bool visible, float seconds)
        {
            Initialize(); if (!runtimeMaterial || !target) return false;
            float destination = visible ? 1 : 0;
            if (!IsAnimating && Mathf.Abs(Visibility - destination) < .0001f) return false;
            if (!IsAnimating && activeCount >= MaxSimultaneousAnimations) { SetVisibleImmediate(visible); return false; }
            if (visible) target.enabled = true;
            if (!IsAnimating) { activeCount++; counted = true; }
            from = Visibility; to = destination; elapsed = 0; duration = Mathf.Max(.05f, seconds); IsAnimating = true; enabled = true; return true;
        }

        void Update()
        {
            if (!IsAnimating) { enabled = false; return; }
            float dt = Time.unscaledDeltaTime; if (!MotionMath.Finite(dt) || dt < 0) dt = 0;
            elapsed += Mathf.Min(dt, .05f); float t = Mathf.Clamp01(elapsed / duration);
            float smooth = t * t * (3 - 2 * t);
            float shaped = to > from ? Mathf.Lerp(smooth, 1 - Mathf.Pow(1 - t, 2.15f), .28f) : Mathf.Lerp(smooth, t * t, .22f);
            Visibility = Mathf.Lerp(from, to, shaped); ApplyVisibility();
            if (t < 1) return;
            bool shown = to > .5f; Visibility = shown ? 1 : 0; ApplyVisibility();
            IsAnimating = false; ReleaseSlot(); target.enabled = shown; enabled = false;
            if (shown) appeared.Invoke(); else disappeared.Invoke(); Completed?.Invoke(this, shown);
        }

        void ApplyVisibility() { if (runtimeMaterial) runtimeMaterial.SetFloat(VisibilityId, Mathf.Clamp01(Visibility)); }
        void ReleaseSlot() { if (!counted) return; counted = false; activeCount = Mathf.Max(0, activeCount - 1); }
        static Vector4 SeedVector(uint value)
        {
            if (value == 0) value = 1;
            return new Vector4(MotionMath.SeededUnit(value + ":a"), MotionMath.SeededUnit(value + ":b"), MotionMath.SeededUnit(value + ":c"), MotionMath.SeededUnit(value + ":d"));
        }
    }
}
