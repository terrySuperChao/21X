using System;
using UnityEngine;

namespace Miscalculation.Motion.Card
{
    /// <summary>业务场景只提供空间锚点；节奏参数不保存屏幕坐标。</summary>
    [Serializable]
    public sealed class CardMotionStage
    {
        public RectTransform coordinateRoot;
        public RectTransform sourceAnchor;
        public RectTransform[] targetSlots;

        public bool TryResolve(int index, out Vector2 start, out Vector2 target)
        {
            start = target = Vector2.zero;
            if (!coordinateRoot || !sourceAnchor || targetSlots == null || index < 0 || index >= targetSlots.Length || !targetSlots[index]) return false;
            start = LocalPivot(sourceAnchor); target = LocalPivot(targetSlots[index]); return true;
        }
        Vector2 LocalPivot(RectTransform value)
        {
            Vector3 p = coordinateRoot.InverseTransformPoint(value.position);
            return new Vector2(p.x, p.y);
        }
    }
}
