using System;
using UnityEngine;

namespace Miscalculation.HallMotion
{
    [CreateAssetMenu(fileName = "HallMotionSettings", menuName = "Miscalculation/Hall Motion Settings")]
    public sealed class HallMotionSettings : ScriptableObject
    {
        public const string MotionVersion = "1.0.7";

        [Header("Web lab parity")]
        [Tooltip("所有背景连续动效的总体幅度倍率，不改变菜单或雨层。")]
        [Range(0f, 1.35f)] public float masterIntensity = 0.82f;
        [Tooltip("漩涡、能量、颗粒等连续背景动画的总体速度倍率。")]
        [Range(0.05f, 1.4f)] public float motionSpeed = 0.38f;
        [Tooltip("中央漩涡的切向扭曲强度；极高值只用于寻找上限，不建议直接作为正式值。")]
        [Range(0f, 3.5f)] public float swirlStrength = 0.64f;
        [Tooltip("中央漩涡的归一化影响半径。只保护最左 Logo/菜单，人物和扑克牌可正常参与。")]
        [Range(0.12f, 0.85f)] public float swirlRadius = 0.31f;
        [Tooltip("漩涡中心附近的低频径向呼吸，不等同于中心二维游移。")]
        [Range(0f, 1.5f)] public float coreBreathStrength = 0.54f;
        [Tooltip("单独增强数学中心附近的二维游移像素，不扩大外围漩涡范围。")]
        [Range(0f, 12f)] public float coreMotionPixels = 3.2f;
        [Tooltip("左侧保护带之外透明彩色主体层的低频墨迹微扰像素。")]
        [Range(0f, 3f)] public float inkWarpPixels = 0.75f;
        [Tooltip("唯一背景保护带的归一化右边界，只保护最左 Logo/菜单；人物和扑克牌不保护。")]
        [Range(0.18f, 0.48f)] public float leftUiProtectWidth = 0.30f;
        [Tooltip("从透明主体原画颜色关系提取的青紫能量增强，不需要控制遮罩。")]
        [Range(0f, 1.5f)] public float energyStrength = 0.67f;
        [Tooltip("鼠标相对屏幕中心造成的最大纯 2D 平面位移像素，不使用 3D 透视。")]
        [Range(0f, 10f)] public float parallaxPixels = 3.2f;
        [Tooltip("复古动态印刷颗粒强度。过高会覆盖原画细节。")]
        [Range(0f, 1.5f)] public float grainStrength = 0.52f;
        [Tooltip("青紫通道的套印错位像素。过高会显得像画面故障。")]
        [Range(0f, 4f)] public float printDriftPixels = 0.9f;
        [Tooltip("0..1 的精神异象出现频率映射；具体随机间隔由下方两组范围插值。")]
        [Range(0f, 1f)] public float anomalyFrequency = 0.48f;

        [Header("Composition")]
        [Tooltip("中央漩涡中心的归一化 UV 坐标。按当前 1920x1080 构图校准为 0.54/0.45。")]
        public Vector2 swirlCenter = new Vector2(0.54f, 0.45f);
        [Tooltip("是否允许随机和手动精神异象。当前产品可根据剧情优先级选择关闭。")]
        public bool anomalyEnabled = true;
        [Tooltip("减少动态效果：控制器会把连续背景幅度降到标准值的 18%。")]
        public bool reducedMotion;
        [Tooltip("调试左侧保护带，仅用于制作验收；正式包必须关闭。")]
        public bool debugMask;

        [Header("Anomaly timing")]
        [Min(0.1f)] public float anomalyDuration = 0.82f;
        public Vector2 anomalyIntervalAtMaxFrequency = new Vector2(13f, 24f);
        public Vector2 anomalyIntervalAtMinFrequency = new Vector2(35f, 62f);

        public static HallMotionSettings CreateRuntimeDefault()
        {
            HallMotionSettings settings = CreateInstance<HallMotionSettings>();
            settings.hideFlags = HideFlags.DontSave;
            return settings;
        }

        public void ApplyJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            HallMotionJson payload = JsonUtility.FromJson<HallMotionJson>(json);
            if (payload == null)
            {
                return;
            }

            masterIntensity = payload.masterIntensity;
            motionSpeed = payload.motionSpeed;
            swirlStrength = payload.swirlStrength;
            swirlRadius = payload.swirlRadius;
            if (string.Equals(payload.schema, "com.miscalculation.hall-motion/v3", StringComparison.Ordinal))
            {
                coreBreathStrength = payload.coreBreathStrength;
                inkWarpPixels = payload.inkWarpPixels;
                if (!string.IsNullOrEmpty(payload.motionVersion))
                {
                    coreMotionPixels = payload.coreMotionPixels;
                }
            }
            leftUiProtectWidth = payload.leftUiProtectWidth > 0f ? payload.leftUiProtectWidth : leftUiProtectWidth;
            energyStrength = payload.energyStrength;
            parallaxPixels = payload.parallaxPixels;
            grainStrength = payload.grainStrength;
            printDriftPixels = payload.printDriftPixels;
            anomalyFrequency = payload.anomalyFrequency;
            swirlCenter = new Vector2(payload.swirlCenterX, payload.swirlCenterY);
            anomalyEnabled = payload.anomalyEnabled;
            reducedMotion = payload.reducedMotion;
            debugMask = payload.debugMask;
            ClampValues();
        }

        public string ToJson(bool prettyPrint = true)
        {
            HallMotionJson payload = HallMotionJson.FromSettings(this);
            return JsonUtility.ToJson(payload, prettyPrint);
        }

        private void OnValidate()
        {
            ClampValues();
        }

        private void ClampValues()
        {
            masterIntensity = Mathf.Clamp(masterIntensity, 0f, 1.35f);
            motionSpeed = Mathf.Clamp(motionSpeed, 0.05f, 1.4f);
            swirlStrength = Mathf.Clamp(swirlStrength, 0f, 3.5f);
            swirlRadius = Mathf.Clamp(swirlRadius, 0.12f, 0.85f);
            coreBreathStrength = Mathf.Clamp(coreBreathStrength, 0f, 1.5f);
            coreMotionPixels = Mathf.Clamp(coreMotionPixels, 0f, 12f);
            inkWarpPixels = Mathf.Clamp(inkWarpPixels, 0f, 3f);
            leftUiProtectWidth = Mathf.Clamp(leftUiProtectWidth, 0.18f, 0.48f);
            energyStrength = Mathf.Clamp(energyStrength, 0f, 1.5f);
            parallaxPixels = Mathf.Clamp(parallaxPixels, 0f, 10f);
            grainStrength = Mathf.Clamp(grainStrength, 0f, 1.5f);
            printDriftPixels = Mathf.Clamp(printDriftPixels, 0f, 4f);
            anomalyFrequency = Mathf.Clamp01(anomalyFrequency);
            swirlCenter.x = Mathf.Clamp01(swirlCenter.x);
            swirlCenter.y = Mathf.Clamp01(swirlCenter.y);
            anomalyDuration = Mathf.Max(0.1f, anomalyDuration);
        }
    }

    [Serializable]
    public sealed class HallMotionJson
    {
        public string schema = "com.miscalculation.hall-motion/v3";
        public string motionVersion = HallMotionSettings.MotionVersion;
        public string renderer = "Unity URP / WebGL2";
        public string aspectRatio = "16:9";
        public float masterIntensity;
        public float motionSpeed;
        public float swirlStrength;
        public float swirlRadius;
        public float coreBreathStrength = 0.54f;
        public float coreMotionPixels = 3.2f;
        public float inkWarpPixels = 0.75f;
        public float leftUiProtectWidth = 0.30f;
        public float energyStrength;
        public float parallaxPixels;
        public float grainStrength;
        public float printDriftPixels;
        public float anomalyFrequency;
        public float swirlCenterX;
        public float swirlCenterY;
        public bool anomalyEnabled;
        public bool reducedMotion;
        public bool debugMask;
        public string preset;

        public static HallMotionJson FromSettings(HallMotionSettings settings)
        {
            return new HallMotionJson
            {
                motionVersion = HallMotionSettings.MotionVersion,
                masterIntensity = settings.masterIntensity,
                motionSpeed = settings.motionSpeed,
                swirlStrength = settings.swirlStrength,
                swirlRadius = settings.swirlRadius,
                coreBreathStrength = settings.coreBreathStrength,
                coreMotionPixels = settings.coreMotionPixels,
                inkWarpPixels = settings.inkWarpPixels,
                leftUiProtectWidth = settings.leftUiProtectWidth,
                energyStrength = settings.energyStrength,
                parallaxPixels = settings.parallaxPixels,
                grainStrength = settings.grainStrength,
                printDriftPixels = settings.printDriftPixels,
                anomalyFrequency = settings.anomalyFrequency,
                swirlCenterX = settings.swirlCenter.x,
                swirlCenterY = settings.swirlCenter.y,
                anomalyEnabled = settings.anomalyEnabled,
                reducedMotion = settings.reducedMotion,
                debugMask = settings.debugMask,
                preset = "custom"
            };
        }
    }
}
