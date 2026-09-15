using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Miscalculation.Motion.Card
{
    /// <summary>One business-owned card and the slot it should replenish.</summary>
    [Serializable]
    public struct CardDealRequest
    {
        public CardView card;
        public int targetSlotIndex;
        public string landingSeed;

        public CardDealRequest(CardView card, int targetSlotIndex, string landingSeed = null)
        {
            this.card = card;
            this.targetSlotIndex = targetSlotIndex;
            this.landingSeed = landingSeed;
        }
    }

    [DisallowMultipleComponent]
    public sealed class CardDealSequence : MonoBehaviour
    {
        public CardMotionPlayer player;
        public CardMotionStage stage;
        public CardView[] cards;
        public bool playerSide = true;
        public bool revealFace = true;
        public IEnumerator Play()
        {
            if (!player || stage == null || cards == null) yield break;
            for (int i = 0; i < cards.Length; i++)
            {
                CardView card = cards[i];
                if (!TryResolve(card, i, out Vector2 start, out Vector2 target)) continue;
                yield return PlayResolved(card, start, target, stage.coordinateRoot.name + ":" + i);
                yield return WaitDealGap();
            }
        }

        /// <summary>
        /// Deals one caller-owned card into an arbitrary configured slot. The caller retains
        /// responsibility for card creation, pooling, removal, hand reflow and gameplay state.
        /// </summary>
        public IEnumerator PlayOne(CardView card, int targetSlotIndex, string landingSeed = null)
        {
            if (!TryResolve(card, targetSlotIndex, out Vector2 start, out Vector2 target)) yield break;
            yield return PlayResolved(card, start, target, ResolveSeed(card, targetSlotIndex, landingSeed));
        }

        /// <summary>
        /// Deals a sparse set of cards to non-contiguous slots, such as one public-card slot
        /// and one hand slot after a level is consumed. Requests are played in list order.
        /// </summary>
        public IEnumerator PlaySubset(IReadOnlyList<CardDealRequest> requests)
        {
            if (!player || stage == null || requests == null) yield break;
            for (int i = 0; i < requests.Count; i++)
            {
                CardDealRequest request = requests[i];
                if (!TryResolve(request.card, request.targetSlotIndex, out Vector2 start, out Vector2 target)) continue;
                yield return PlayResolved(
                    request.card,
                    start,
                    target,
                    ResolveSeed(request.card, request.targetSlotIndex, request.landingSeed));
                yield return WaitDealGap();
            }
        }

        bool TryResolve(CardView card, int targetSlotIndex, out Vector2 start, out Vector2 target)
        {
            start = target = Vector2.zero;
            return player && stage != null && card && card.motionRoot
                && stage.TryResolve(targetSlotIndex, out start, out target);
        }

        IEnumerator PlayResolved(CardView card, Vector2 start, Vector2 target, string landingSeed)
        {
            card.gameObject.SetActive(true);
            card.ShowBack();
            yield return player.PlayDealRoutine(
                card.motionRoot,
                start,
                target,
                playerSide,
                revealFace,
                card.ShowFace,
                0f,
                landingSeed,
                null,
                card.impactVisual);
        }

        IEnumerator WaitDealGap()
        {
            float gap = player.Profile.sequence.dealGapMs * .001f;
            while (gap > 0f) { gap -= Time.unscaledDeltaTime; yield return null; }
        }

        string ResolveSeed(CardView card, int targetSlotIndex, string requested)
        {
            if (!string.IsNullOrEmpty(requested)) return requested;
            string rootName = stage.coordinateRoot ? stage.coordinateRoot.name : "CardStage";
            return rootName + ":slot:" + targetSlotIndex + ":card:" + card.GetInstanceID();
        }
    }
}
