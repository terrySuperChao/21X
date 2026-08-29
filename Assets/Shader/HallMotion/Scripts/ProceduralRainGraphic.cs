using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Miscalculation.HallMotion
{
    /// <summary>
    /// Two-layer deterministic 2D rain. Put it above the hall background and below logo/menu UI.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ProceduralRainGraphic : MaskableGraphic
    {
        [SerializeField] private HallRainSettings settings;
        [SerializeField] private bool rainEnabled = true;
        [SerializeField] private bool reducedMotion;
        [SerializeField] private bool useUnscaledTime = true;

        private readonly List<RainDrop> drops = new List<RainDrop>(220);
        private int generatedSeed;
        private int generatedCount;
        private float generatedNearRatio;

        /// <summary>当前受 220 条硬上限约束的雨丝数量，供性能验收和调试面板只读显示。</summary>
        public int GeneratedDropCount => drops.Count;

        private float Clock => useUnscaledTime ? Time.unscaledTime : Time.time;

        protected override void Awake()
        {
            base.Awake();
            raycastTarget = false;
            color = Color.white;
            if (settings == null) settings = HallRainSettings.CreateRuntimeDefault();
            Generate();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Generate();
        }

        private void Update()
        {
            if (!rainEnabled || settings == null) return;
            int expectedCount = Mathf.Max(0, Mathf.RoundToInt(24f + settings.density * 196f));
            if (settings.seed != generatedSeed || expectedCount != generatedCount || !Mathf.Approximately(settings.nearRatio, generatedNearRatio))
            {
                Generate();
            }
            SetVerticesDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
        }

        public void SetRainEnabled(bool value)
        {
            rainEnabled = value;
            canvasRenderer.cull = !value;
            if (value) SetVerticesDirty();
        }

        public void SetReducedMotion(bool value)
        {
            reducedMotion = value;
        }

        public void ApplyJson(TextAsset jsonConfig)
        {
            if (jsonConfig == null) return;
            HallRainRootJson root = JsonUtility.FromJson<HallRainRootJson>(jsonConfig.text);
            settings.ApplyWebJson(jsonConfig.text);
            if (root != null && root.rain != null) SetRainEnabled(root.rain.enabled);
            Generate();
        }

        public void Refresh()
        {
            Generate();
            SetVerticesDirty();
        }

        private void Generate()
        {
            drops.Clear();
            if (settings == null) return;
            int count = Mathf.Max(0, Mathf.RoundToInt(24f + settings.density * 196f));
            int nearCount = Mathf.RoundToInt(count * settings.nearRatio);
            XorShift32 random = new XorShift32(unchecked((uint)settings.seed));
            for (int index = 0; index < count; index++)
            {
                bool near = index >= count - nearCount;
                drops.Add(new RainDrop
                {
                    near = near,
                    x = (float)random.Next(),
                    y = (float)random.Next(),
                    speedScale = (near ? 1.12f : 0.52f) + (float)random.Next() * (near ? 0.44f : 0.30f),
                    lengthScale = (near ? 1.18f : 0.58f) + (float)random.Next() * (near ? 0.62f : 0.34f),
                    alphaScale = (near ? 0.66f : 0.26f) + (float)random.Next() * (near ? 0.34f : 0.30f),
                    width = (near ? 1.0f : 0.45f) + (float)random.Next() * (near ? 0.85f : 0.42f),
                    phase = (float)random.Next()
                });
            }
            generatedSeed = settings.seed;
            generatedCount = count;
            generatedNearRatio = settings.nearRatio;
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
            if (!rainEnabled || settings == null || drops.Count == 0) return;
            Rect rect = GetPixelAdjustedRect();
            float width = Mathf.Max(1f, rect.width);
            float height = Mathf.Max(1f, rect.height);
            float elapsed = Clock;
            float motionReduction = reducedMotion ? 0.18f : 1f;
            float alphaReduction = reducedMotion ? 0.62f : 1f;
            float referenceScale = height / 1080f;
            float angle = settings.angleDeg * Mathf.Deg2Rad;
            float gust = GustAt(elapsed) * motionReduction;
            float speedBoost = 1f + gust * settings.gustStrength;
            float windBoost = 1f + gust * settings.gustStrength * 1.45f;
            float margin = Mathf.Max(40f, settings.length * referenceScale * 3f);

            for (int index = 0; index < drops.Count; index++)
            {
                RainDrop drop = drops[index];
                float dropSpeed = settings.speed * referenceScale * drop.speedScale * motionReduction * speedBoost;
                float dropLength = settings.length * referenceScale * drop.lengthScale;
                float drift = (settings.wind * referenceScale * windBoost + Mathf.Tan(angle) * dropSpeed * 0.32f) * motionReduction;
                float spanY = height + margin * 2f;
                float spanX = width + margin * 2f;
                float y = Mod(drop.y * spanY + elapsed * dropSpeed + drop.phase * margin, spanY) - margin;
                float x = Mod(drop.x * spanX + elapsed * drift + drop.phase * width * 0.13f, spanX) - margin;
                float dx = Mathf.Sin(angle) * dropLength + (settings.wind / Mathf.Max(80f, settings.speed)) * dropLength * 0.38f;
                float dy = Mathf.Cos(angle) * dropLength;
                float protect = ProtectionAt(x / width, y / height);
                float alpha = settings.opacity * drop.alphaScale * protect * alphaReduction;
                if (alpha <= 0.002f) continue;

                Vector2 end = new Vector2(rect.xMin + x, rect.yMax - y);
                Vector2 start = end + new Vector2(-dx, dy);
                Color coreColor = drop.near
                    ? new Color(180f / 255f, 230f / 255f, 238f / 255f, alpha)
                    : new Color(113f / 255f, 161f / 255f, 179f / 255f, alpha);
                if (drop.near && settings.glow > 0f)
                {
                    Color glowColor = new Color(117f / 255f, 251f / 255f, 246f / 255f, alpha * 0.12f);
                    AddLine(vertexHelper, start, end, drop.width * referenceScale + settings.glow * referenceScale, glowColor);
                }
                AddLine(vertexHelper, start, end, drop.width * referenceScale, coreColor);
            }
        }

        private float ProtectionAt(float x, float y)
        {
            float menuHorizontal = 1f - SmoothStep(0.17f, 0.39f, x);
            float menuVertical = SmoothStep(0.03f, 0.18f, y) * (1f - SmoothStep(0.92f, 1.04f, y));
            float menuMask = menuHorizontal * menuVertical;
            float dx = (x - 0.55f) / 0.27f;
            float dy = (y - 0.49f) / 0.25f;
            float coreMask = Mathf.Max(0f, 1f - Mathf.Sqrt(dx * dx + dy * dy));
            return Mathf.Max(0.08f, (1f - menuMask * settings.menuProtect) * (1f - coreMask * settings.coreProtect));
        }

        private float GustAt(float elapsed)
        {
            if (settings.gustChance <= 0f) return 0f;
            float period = 18f - settings.gustChance * 14f;
            float phase = (settings.seed % 997) / 997f * Mathf.PI * 2f;
            float wave = Mathf.Sin(elapsed / Mathf.Max(2f, period) * Mathf.PI * 2f + phase);
            float threshold = 0.92f - settings.gustChance * 0.30f;
            float normalized = Mathf.Max(0f, (wave - threshold) / Mathf.Max(0.01f, 1f - threshold));
            return normalized * normalized;
        }

        private static float SmoothStep(float edge0, float edge1, float value)
        {
            float t = Mathf.Clamp01((value - edge0) / Mathf.Max(0.0001f, edge1 - edge0));
            return t * t * (3f - 2f * t);
        }

        private static float Mod(float value, float divisor)
        {
            return ((value % divisor) + divisor) % divisor;
        }

        private static void AddLine(VertexHelper helper, Vector2 start, Vector2 end, float thickness, Color lineColor)
        {
            Vector2 direction = (end - start).normalized;
            if (direction.sqrMagnitude < 0.0001f) return;
            Vector2 normal = new Vector2(-direction.y, direction.x) * Mathf.Max(0.1f, thickness) * 0.5f;
            int baseIndex = helper.currentVertCount;
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = lineColor;
            vertex.position = start + normal;
            helper.AddVert(vertex);
            vertex.position = start - normal;
            helper.AddVert(vertex);
            vertex.position = end + normal;
            helper.AddVert(vertex);
            vertex.position = end - normal;
            helper.AddVert(vertex);
            helper.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 1);
            helper.AddTriangle(baseIndex + 2, baseIndex + 3, baseIndex + 1);
        }

        private struct RainDrop
        {
            public bool near;
            public float x;
            public float y;
            public float speedScale;
            public float lengthScale;
            public float alphaScale;
            public float width;
            public float phase;
        }

        private struct XorShift32
        {
            private uint value;
            public XorShift32(uint seed)
            {
                value = seed == 0u ? 0x6d2b79f5u : seed;
            }
            public double Next()
            {
                uint x = value;
                x ^= x << 13;
                x ^= x >> 17;
                x ^= x << 5;
                value = x;
                return value / 4294967296.0;
            }
        }
    }
}
