using System;
using UnityEngine;
using UnityEngine.UI;

namespace Miscalculation.Motion.Common
{
    [DisallowMultipleComponent]
    public sealed class ImageRebuildTransition : MonoBehaviour
    {
        public Image sourceLayer;
        public Image targetLayer;
        public MemoryImageEffect sourceEffect;
        public MemoryImageEffect targetEffect;
        public event Action Completed;
        MemoryTransitionSettings settings = new MemoryTransitionSettings();
        bool running;

        public void Configure(MemoryTransitionSettings value)
        {
            settings = value ?? new MemoryTransitionSettings(); settings.Validate();
            if (sourceEffect) sourceEffect.ApplySettings(settings);
            if (targetEffect) targetEffect.ApplySettings(settings);
        }

        public bool Rebuild(Sprite from, Sprite to, uint stableSeed = 1)
        {
            if (!sourceLayer || !targetLayer || !sourceEffect || !targetEffect || !to) return false;
            sourceEffect.Completed -= OnSourceComplete; targetEffect.Completed -= OnTargetComplete;
            sourceEffect.seed = stableSeed; targetEffect.seed = stableSeed;
            sourceEffect.ApplySettings(settings); targetEffect.ApplySettings(settings);
            sourceLayer.sprite = from ? from : sourceLayer.sprite; targetLayer.sprite = to;
            sourceEffect.SetVisibleImmediate(true); targetEffect.SetVisibleImmediate(false);
            sourceEffect.Completed -= OnSourceComplete; sourceEffect.Completed += OnSourceComplete;
            sourceEffect.PlayTo(false, settings.rebuildDurationMs * .00045f);
            running = true; return true;
        }

        public void SetImmediate(Sprite value)
        {
            running = false; sourceEffect.Completed -= OnSourceComplete; targetEffect.Completed -= OnTargetComplete;
            if (sourceLayer && value) sourceLayer.sprite = value;
            if (sourceEffect) sourceEffect.SetVisibleImmediate(true);
            if (targetEffect) targetEffect.SetVisibleImmediate(false);
        }

        void OnSourceComplete(MemoryImageEffect _, bool visible)
        {
            if (!running || visible) return;
            sourceEffect.Completed -= OnSourceComplete;
            targetEffect.Completed -= OnTargetComplete; targetEffect.Completed += OnTargetComplete;
            targetEffect.PlayTo(true, settings.rebuildDurationMs * .00055f);
        }

        void OnTargetComplete(MemoryImageEffect _, bool visible)
        {
            if (!running || !visible) return;
            targetEffect.Completed -= OnTargetComplete; running = false;
            sourceLayer.sprite = targetLayer.sprite;
            sourceEffect.SetVisibleImmediate(true); targetEffect.SetVisibleImmediate(false);
            Completed?.Invoke();
        }
    }
}
