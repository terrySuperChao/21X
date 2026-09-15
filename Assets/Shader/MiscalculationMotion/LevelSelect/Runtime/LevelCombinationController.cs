using System.Collections;
using System.Collections.Generic;
using Miscalculation.Motion.Common;
using UnityEngine;

namespace Miscalculation.Motion.LevelSelect
{
    [DisallowMultipleComponent]
    public sealed class LevelCombinationController : MonoBehaviour
    {
        [System.Serializable] public sealed class Mapping { public LevelCardDropTarget target; public Sprite combinedArtwork; }
        public LevelSelectMotionProfile profile = new LevelSelectMotionProfile();
        public Mapping[] mappings;
        readonly Dictionary<LevelCardDraggable, Mapping> active = new Dictionary<LevelCardDraggable, Mapping>(8);
        readonly HashSet<LevelCardDraggable> registered = new HashSet<LevelCardDraggable>();
        readonly Dictionary<LevelCardDraggable, Coroutine> pending = new Dictionary<LevelCardDraggable, Coroutine>(8);
        public void Register(LevelCardDraggable card)
        {
            if (!card) return; registered.Add(card); card.Attached -= OnAttached; card.Detached -= OnDetached; card.Attached += OnAttached; card.Detached += OnDetached;
        }
        public void ResetImmediate()
        {
            StopAllCoroutines(); pending.Clear(); active.Clear();
            foreach (LevelCardDraggable card in registered) if (card) card.SetArtworkImmediate();
            if (mappings != null)
                for (int i = 0; i < mappings.Length; i++)
                    if (mappings[i] != null && mappings[i].target)
                        mappings[i].target.ResetFeedbackImmediate();
        }
        void OnAttached(LevelCardDraggable card, LevelCardDropTarget target)
        {
            Mapping map = Find(target); if (map == null) return;
            profile.Validate();
            // Attached fires after the dragged card has settled, keeping confirmation feedback
            // separate from the drag/snap transform and from the optional artwork rebuild.
            target.PlayConfirmPulse(card.visualRoot, profile.validPulseScale, profile.validPulseDurationMs);
            active[card] = map; Queue(card, map, true);
        }
        void OnDetached(LevelCardDraggable card, LevelCardDropTarget target)
        {
            if (target) target.ResetFeedbackImmediate();
            if (!active.TryGetValue(card, out Mapping map)) return; active.Remove(card); Queue(card, map, false);
        }
        void Queue(LevelCardDraggable card, Mapping map, bool combined)
        {
            if (pending.TryGetValue(card, out Coroutine routine) && routine != null) StopCoroutine(routine);
            pending[card] = StartCoroutine(RebuildAfterDelay(card, map, combined));
        }
        IEnumerator RebuildAfterDelay(LevelCardDraggable card, Mapping map, bool combined)
        {
            float wait = profile.rebuildDelayMs * .001f; while (wait > 0) { wait -= Time.unscaledDeltaTime; yield return null; }
            pending.Remove(card); if (!card) yield break;
            if (combined)
            {
                if (!active.TryGetValue(card, out Mapping current) || current != map) yield break;
                card.RebuildArtwork(card.baseArtwork, map.combinedArtwork, StableSeed(card, map));
            }
            else
            {
                if (active.ContainsKey(card)) yield break;
                card.RebuildArtwork(map.combinedArtwork, card.baseArtwork, StableSeed(card, map));
            }
        }
        static uint StableSeed(LevelCardDraggable card, Mapping map) => (uint)((card.GetInstanceID() * 397 ^ map.target.GetInstanceID()) & 0x7fffffff);
        Mapping Find(LevelCardDropTarget target) { if (mappings != null) for (int i = 0; i < mappings.Length; i++) if (mappings[i] != null && mappings[i].target == target) return mappings[i]; return null; }
    }
}
