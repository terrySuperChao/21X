using System;
using UnityEngine;

namespace Miscalculation.HallMotion
{
    [CreateAssetMenu(fileName = "HallRainSettings", menuName = "Miscalculation/Hall Rain Settings")]
    public sealed class HallRainSettings : ScriptableObject
    {
        public const string AlgorithmVersion = "xorshift32-dual-rain-v2-monotonic-gust";

        [Tooltip("雨丝固定随机种子；相同 Seed、密度和近景比例生成同一基础集合。")]
        [Range(1, 9999)] public int seed = 1997;
        [Tooltip("雨丝密度 0..1，对应总数约 24..220 条，只有变化时才重建集合。")]
        [Range(0f, 1f)] public float density = 0.38f;
        [Tooltip("总雨丝中近景亮雨的比例；其余为更短、更暗、更慢的远景雨。")]
        [Range(0.05f, 0.65f)] public float nearRatio = 0.28f;
        [Tooltip("基础雨丝长度像素，远近景会再乘各自受限随机倍率。")]
        [Range(6f, 54f)] public float length = 24f;
        [Tooltip("基础下落速度像素/秒，使用 UI 本地坐标。")]
        [Range(80f, 900f)] public float speed = 460f;
        [Tooltip("雨丝相对竖直方向的倾斜角度。")]
        [Range(-24f, 24f)] public float angleDeg = 9f;
        [Tooltip("整体水平风速像素/秒，正值向右。")]
        [Range(-220f, 220f)] public float wind = 35f;
        [Tooltip("雨层总体透明度；近远景仍会分别乘自己的透明度倍率。")]
        [Range(0.02f, 0.55f)] public float opacity = 0.24f;
        [Tooltip("近景雨丝的附加辉光宽度。")]
        [Range(0f, 8f)] public float glow = 1.5f;
        [Tooltip("左侧菜单区域的雨丝透明度保护强度，1 为最大抑制但不会硬删除雨丝。")]
        [Range(0f, 1f)] public float menuProtect = 0.76f;
        [Tooltip("中央漩涡构图区域的雨丝透明度保护强度，避免亮雨盖住视觉中心。")]
        [Range(0f, 1f)] public float coreProtect = 0.48f;
        [Tooltip("低频阵风触发概率。阵风只改变受控运动参数，不重建无上限粒子。")]
        [Range(0f, 1f)] public float gustChance = 0.18f;
        [Tooltip("阵风对速度与横向风的附加强度。")]
        [Range(0f, 0.8f)] public float gustStrength = 0.30f;

        public static HallRainSettings CreateRuntimeDefault()
        {
            HallRainSettings settings = CreateInstance<HallRainSettings>();
            settings.hideFlags = HideFlags.DontSave;
            return settings;
        }

        public void ApplyWebJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;
            HallRainRootJson root = JsonUtility.FromJson<HallRainRootJson>(json);
            if (root == null || root.rain == null) return;
            HallRainJson data = root.rain;
            seed = data.seed;
            density = data.density;
            nearRatio = data.nearRatio;
            length = data.length;
            speed = data.speed;
            angleDeg = data.angleDeg;
            wind = data.wind;
            opacity = data.opacity;
            glow = data.glow;
            menuProtect = data.menuProtect;
            coreProtect = data.coreProtect;
            gustChance = data.gustChance;
            gustStrength = data.gustStrength;
            ClampValues();
        }

        private void OnValidate()
        {
            ClampValues();
        }

        private void ClampValues()
        {
            seed = Mathf.Clamp(seed, 1, 9999);
            density = Mathf.Clamp01(density);
            nearRatio = Mathf.Clamp(nearRatio, 0.05f, 0.65f);
            length = Mathf.Clamp(length, 6f, 54f);
            speed = Mathf.Clamp(speed, 80f, 900f);
            angleDeg = Mathf.Clamp(angleDeg, -24f, 24f);
            wind = Mathf.Clamp(wind, -220f, 220f);
            opacity = Mathf.Clamp(opacity, 0.02f, 0.55f);
            glow = Mathf.Clamp(glow, 0f, 8f);
            menuProtect = Mathf.Clamp01(menuProtect);
            coreProtect = Mathf.Clamp01(coreProtect);
            gustChance = Mathf.Clamp01(gustChance);
            gustStrength = Mathf.Clamp(gustStrength, 0f, 0.8f);
        }
    }

    [Serializable]
    public sealed class HallRainRootJson
    {
        public HallRainJson rain;
    }

    [Serializable]
    public sealed class HallRainJson
    {
        public bool enabled = true;
        public int seed;
        public float density;
        public float nearRatio;
        public float length;
        public float speed;
        public float angleDeg;
        public float wind;
        public float opacity;
        public float glow;
        public float menuProtect;
        public float coreProtect;
        public float gustChance;
        public float gustStrength;
        public string algorithm;
    }
}
