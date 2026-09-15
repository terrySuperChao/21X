using UnityEngine;
using UnityEngine.UI;

namespace Miscalculation.Motion.Card
{
    [DisallowMultipleComponent]
    public sealed class CardView : MonoBehaviour
    {
        public RectTransform motionRoot;
        public RectTransform impactVisual;
        public Image faceImage;
        public Image backImage;
        public void ShowBack() { if (backImage) backImage.enabled = true; if (faceImage) faceImage.enabled = false; }
        public void ShowFace() { if (backImage) backImage.enabled = false; if (faceImage) faceImage.enabled = true; }
    }
}
