using System;
using System.Collections.Generic;
using UnityEngine;

namespace Duel21.FeelLab
{
    /// <summary>
    /// Adds presentation effects around CardMotionFeel without modifying the
    /// replaceable motion implementation.
    /// </summary>
    public sealed class CardMotionDustBinder : MonoBehaviour
    {
        private struct PendingLanding
        {
            public RectTransform card;
            public string seed;
        }

        [SerializeField] private CardMotionFeel motion;
        [SerializeField] private CardDustEmitter dustEmitter;
        [SerializeField] private FeelProfileV1.EffectsSettings effects = new FeelProfileV1.EffectsSettings();
        private readonly Queue<PendingLanding> pendingLandings = new Queue<PendingLanding>();
        private int burstSerial;

        private void OnEnable()
        {
            if (motion != null)
            {
                motion.Cue += OnMotionCue;
            }
        }

        private void OnDisable()
        {
            if (motion != null)
            {
                motion.Cue -= OnMotionCue;
            }
            pendingLandings.Clear();
        }

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
            if (motion == null || card == null)
            {
                return null;
            }

            string seed = landingSeed ?? $"{(playerSide ? "player" : "npc")}:{Mathf.RoundToInt(target.x)}:{Mathf.RoundToInt(target.y)}";
            pendingLandings.Enqueue(new PendingLanding { card = card, seed = seed });
            return motion.PlayDeal(
                card,
                start,
                target,
                playerSide,
                revealFace,
                swapToFace,
                targetRotation,
                seed,
                applyEdgePeel,
                impactVisual);
        }

        private void OnMotionCue(CardMotionCue cue)
        {
            if (cue != CardMotionCue.Land || pendingLandings.Count == 0)
            {
                return;
            }

            PendingLanding landing = pendingLandings.Dequeue();
            if (landing.card == null || dustEmitter == null)
            {
                return;
            }

            dustEmitter.Emit(landing.card, CreateSample(landing.seed), effects);
        }

        private CardDustSample CreateSample(string cardSeed)
        {
            burstSerial += 1;
            float variation = Mathf.Clamp01(effects.dustVariation);
            string seed = $"{cardSeed}:dust:{burstSerial}:{Time.unscaledTime:F6}";
            int baseCount = Mathf.Max(0, Mathf.RoundToInt(effects.dustCount));
            int countRadius = baseCount > 0 && variation > 0f
                ? Mathf.Max(1, Mathf.RoundToInt(baseCount * 0.38f * variation))
                : 0;
            int countOffset = countRadius > 0
                ? Mathf.RoundToInt((CardDustEmitter.SeededUnit(seed + ":count") * 2f - 1f) * countRadius)
                : 0;
            return new CardDustSample
            {
                seed = seed,
                count = Mathf.Max(0, baseCount + countOffset),
                edgeOffset = Mathf.FloorToInt(CardDustEmitter.SeededUnit(seed + ":edge") * 4f),
                spreadScale = 1f + (CardDustEmitter.SeededUnit(seed + ":spread") * 2f - 1f) * 0.46f * variation,
                liftScale = 1f + (CardDustEmitter.SeededUnit(seed + ":lift") * 2f - 1f) * 0.36f * variation,
                directionBiasDeg = (CardDustEmitter.SeededUnit(seed + ":direction") * 2f - 1f) * 18f * variation,
                durationScale = 1f + (CardDustEmitter.SeededUnit(seed + ":duration") * 2f - 1f) * 0.16f * variation,
                markScale = 1f + (CardDustEmitter.SeededUnit(seed + ":mark-scale") * 2f - 1f) * 0.1f * variation
            };
        }
    }
}
