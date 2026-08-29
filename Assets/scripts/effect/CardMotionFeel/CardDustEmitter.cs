using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Duel21.FeelLab
{
    public struct CardDustSample
    {
        public string seed;
        public int count;
        public int edgeOffset;
        public float spreadScale;
        public float liftScale;
        public float directionBiasDeg;
        public float durationScale;
        public float markScale;
    }

    /// <summary>Lightweight pooled UI dust for Screen Space canvases.</summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class CardDustEmitter : MonoBehaviour
    {
        [SerializeField] private Color dustColor = new Color(0.88f, 0.79f, 0.64f, 1f);
        [SerializeField, Min(1f)] private float minimumSize = 5f;
        [SerializeField, Min(1f)] private float maximumSize = 13f;

        private readonly Stack<Image> pool = new Stack<Image>();
        private RectTransform layer;
        private static Sprite dustSprite;

        private void Awake()
        {
            layer = (RectTransform)transform;
        }

        public void Emit(
            RectTransform card,
            CardDustSample sample,
            FeelProfileV1.EffectsSettings settings)
        {
            if (card == null || settings == null || sample.count <= 0)
            {
                return;
            }

            if (layer == null)
            {
                layer = (RectTransform)transform;
            }

            Vector3[] worldCorners = new Vector3[4];
            card.GetWorldCorners(worldCorners);
            Vector2[] corners = new Vector2[4];
            for (int index = 0; index < corners.Length; index++)
            {
                corners[index] = layer.InverseTransformPoint(worldCorners[index]);
            }

            for (int index = 0; index < sample.count; index++)
            {
                StartCoroutine(PlayParticle(index, corners, sample, settings));
            }
        }

        private IEnumerator PlayParticle(
            int index,
            Vector2[] corners,
            CardDustSample sample,
            FeelProfileV1.EffectsSettings settings)
        {
            Image image = Rent();
            RectTransform rect = image.rectTransform;
            string key = $"{sample.seed}:{index}";
            float along = SeededUnit(key + ":along");
            float side = SeededUnit(key + ":side") < 0.5f ? -1f : 1f;
            float spread = Mathf.Lerp(0.35f, 1f, SeededUnit(key + ":spread"));
            float lift = Mathf.Lerp(0.45f, 1f, SeededUnit(key + ":lift"));
            float size = Mathf.Lerp(minimumSize, maximumSize, SeededUnit(key + ":size"));
            float rotation = SeededUnit(key + ":rotation") * 180f;

            int edge = (index + sample.edgeOffset) % 4;
            int edgeStart;
            int edgeEnd;
            Vector2 outward;
            switch (edge)
            {
                case 0: // bottom
                    edgeStart = 0; edgeEnd = 3; outward = Vector2.down;
                    break;
                case 1: // right
                    edgeStart = 3; edgeEnd = 2; outward = Vector2.right;
                    break;
                case 2: // top
                    edgeStart = 2; edgeEnd = 1; outward = Vector2.up;
                    break;
                default: // left
                    edgeStart = 1; edgeEnd = 0; outward = Vector2.left;
                    break;
            }

            Vector2 tangent = (corners[edgeEnd] - corners[edgeStart]).normalized;
            Vector2 start = Vector2.Lerp(corners[edgeStart], corners[edgeEnd], along);
            start += outward * Mathf.Lerp(2f, 7f, SeededUnit(key + ":edge-gap"));
            float directionRadians = sample.directionBiasDeg * Mathf.Deg2Rad;
            Vector2 direction = (
                outward * Mathf.Cos(directionRadians)
                + tangent * side * Mathf.Sin(directionRadians + 0.38f)).normalized;
            Vector2 end = start
                + direction * settings.dustSpreadPx * sample.spreadScale * spread
                + outward * settings.dustLiftPx * sample.liftScale * lift * 0.5f;

            rect.anchoredPosition = start;
            rect.sizeDelta = Vector2.one * size;
            rect.localEulerAngles = new Vector3(0f, 0f, rotation);
            rect.localScale = Vector3.one * sample.markScale;
            image.color = new Color(dustColor.r, dustColor.g, dustColor.b, settings.dustOpacity);
            image.gameObject.SetActive(true);

            float duration = Mathf.Max(0.05f, settings.dustDurationMs * sample.durationScale / 1000f);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                float arc = Mathf.Sin(t * Mathf.PI) * settings.dustLiftPx * 0.18f;
                rect.anchoredPosition = Vector2.LerpUnclamped(start, end, eased) + outward * arc;
                rect.localEulerAngles = new Vector3(0f, 0f, rotation + side * 95f * t);
                rect.localScale = Vector3.one * sample.markScale * Mathf.Lerp(1f, 0.18f, eased);
                float alpha = settings.dustOpacity * (1f - t) * (1f - t);
                image.color = new Color(dustColor.r, dustColor.g, dustColor.b, alpha);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            Return(image);
        }

        private Image Rent()
        {
            if (pool.Count > 0)
            {
                return pool.Pop();
            }

            GameObject particle = new GameObject("Dust", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            particle.layer = gameObject.layer;
            RectTransform rect = particle.GetComponent<RectTransform>();
            rect.SetParent(layer, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            Image image = particle.GetComponent<Image>();
            image.raycastTarget = false;
            image.sprite = GetDustSprite();
            image.preserveAspect = true;
            return image;
        }

        private void Return(Image image)
        {
            image.gameObject.SetActive(false);
            pool.Push(image);
        }

        private static Sprite GetDustSprite()
        {
            if (dustSprite != null)
            {
                return dustSprite;
            }

            const int size = 24;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = "Card Dust (Runtime)",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave
            };
            Color[] pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 p = new Vector2(x, y) / (size - 1f) * 2f - Vector2.one;
                    float alpha = Mathf.SmoothStep(1f, 0f, p.magnitude);
                    pixels[y * size + x] = new Color(1f, 1f, 1f, alpha * alpha);
                }
            }
            texture.SetPixels(pixels);
            texture.Apply(false, true);
            dustSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), Vector2.one * 0.5f, size);
            dustSprite.name = "Card Dust (Runtime)";
            dustSprite.hideFlags = HideFlags.DontSave;
            return dustSprite;
        }

        internal static float SeededUnit(string value)
        {
            unchecked
            {
                uint hash = 2166136261u;
                string text = value ?? string.Empty;
                for (int index = 0; index < text.Length; index++)
                {
                    hash ^= text[index];
                    hash *= 16777619u;
                }
                return hash / 4294967296f;
            }
        }
    }
}
