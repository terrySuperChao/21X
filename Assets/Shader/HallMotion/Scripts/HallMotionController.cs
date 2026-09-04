using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Miscalculation.HallMotion
{
    /// <summary>
    /// 驱动 1920x1080 全屏 RawImage 的单材质 2D 主界面背景动效。
    /// 挂在显示 hall-bg-black.png 的 RawImage 上，透明彩色主体通过 Art Texture 传给 Shader。
    /// C# 只更新少量材质参数，不做逐像素读取；运行时材质只在 Awake 复制一次，避免污染共享资产。
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public sealed class HallMotionController : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
    {
        [Tooltip("背景动效参数资产。留空时会创建不保存的运行时默认值；正式场景建议显式赋值。")]
        [SerializeField] private HallMotionSettings settings;
        [Tooltip("使用 Miscalculation/UI/Hall Motion URP 的共享模板材质。控制器会复制运行时实例，不会修改原资产。")]
        [SerializeField] private Material sourceMaterial;
        [Tooltip("1920x1080 透明彩色主体层 hall-art-overlay.png，必须保留 Alpha、sRGB On、Clamp、Mip Maps Off。")]
        [SerializeField] private Texture artTexture;
        [Tooltip("开启时在暂停菜单也继续播放主界面环境动效；标准接入保持开启。")]
        [SerializeField] private bool useUnscaledTime = true;
        [Tooltip("旧输入管理器可用时，每帧以零分配方式补采鼠标位置。它让背景视差在指针越过按钮、Game 视图刚获得焦点等情况下仍连续；使用纯新 Input System 的项目会自动只走 IPointerMove。")]
        [SerializeField] private bool sampleLegacyMousePosition = true;
        [Tooltip("低频精神异象的固定随机序列种子，便于回归同一节奏。")]
        [SerializeField] private int randomSeed = 1997;

        private RawImage targetImage;
        private Canvas targetCanvas;
        private Material runtimeMaterial;
        private Vector2 pointerTarget;
        private Vector2 pointerCurrent;
        private float nextAnomalyTime;
        private float anomalyValue;
        private Coroutine anomalyRoutine;
        private System.Random anomalyRandom;

        private static readonly int ResolutionId = Shader.PropertyToID("_Resolution");
        private static readonly int PointerId = Shader.PropertyToID("_Pointer");
        private static readonly int SwirlCenterId = Shader.PropertyToID("_SwirlCenter");
        private static readonly int MotionTimeId = Shader.PropertyToID("_MotionTime");
        private static readonly int MasterId = Shader.PropertyToID("_MasterIntensity");
        private static readonly int SpeedId = Shader.PropertyToID("_MotionSpeed");
        private static readonly int SwirlId = Shader.PropertyToID("_SwirlStrength");
        private static readonly int RadiusId = Shader.PropertyToID("_SwirlRadius");
        private static readonly int CoreBreathId = Shader.PropertyToID("_CoreBreathStrength");
        private static readonly int CoreMotionId = Shader.PropertyToID("_CoreMotionPixels");
        private static readonly int InkWarpId = Shader.PropertyToID("_InkWarpPixels");
        private static readonly int ArtTextureId = Shader.PropertyToID("_ArtTex");
        private static readonly int LeftUiProtectWidthId = Shader.PropertyToID("_LeftUiProtectWidth");
        private static readonly int EnergyId = Shader.PropertyToID("_EnergyStrength");
        private static readonly int ParallaxId = Shader.PropertyToID("_ParallaxPixels");
        private static readonly int GrainId = Shader.PropertyToID("_GrainStrength");
        private static readonly int PrintDriftId = Shader.PropertyToID("_PrintDriftPixels");
        private static readonly int AnomalyId = Shader.PropertyToID("_Anomaly");
        private static readonly int DebugMaskId = Shader.PropertyToID("_DebugMask");

        public HallMotionSettings Settings => settings;

        /// <summary>当前平滑前的归一化指针目标，供验证场景和诊断面板只读检查。</summary>
        public Vector2 PointerTarget => pointerTarget;

        private float Clock => useUnscaledTime ? Time.unscaledTime : Time.time;
        private float DeltaTime => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        private void Awake()
        {
            targetImage = GetComponent<RawImage>();
            targetCanvas = targetImage.canvas;
            if (settings == null)
            {
                settings = HallMotionSettings.CreateRuntimeDefault();
            }

            Material template = sourceMaterial != null ? sourceMaterial : targetImage.material;
            if (template == null || template.shader == null || template.shader.name != "Miscalculation/UI/Hall Motion URP")
            {
                Shader shader = Shader.Find("Miscalculation/UI/Hall Motion URP");
                if (shader == null)
                {
                    Debug.LogError("Hall Motion URP shader was not found.", this);
                    enabled = false;
                    return;
                }

                template = new Material(shader);
            }

            runtimeMaterial = new Material(template)
            {
                name = $"{template.name} (Runtime)",
                hideFlags = HideFlags.DontSave
            };
            if (artTexture != null)
            {
                runtimeMaterial.SetTexture(ArtTextureId, artTexture);
            }
            targetImage.material = runtimeMaterial;
            anomalyRandom = new System.Random(randomSeed);
            ScheduleNextAnomaly();
        }

        private void OnDestroy()
        {
            if (runtimeMaterial != null)
            {
                Destroy(runtimeMaterial);
            }
        }

        private void Update()
        {
            if (runtimeMaterial == null)
            {
                return;
            }

            float reduction = settings.reducedMotion ? 0.18f : 1f;
#if ENABLE_LEGACY_INPUT_MANAGER
            if (sampleLegacyMousePosition && Input.mousePresent)
            {
                TrySetPointerTarget(Input.mousePosition, ResolveEventCamera());
            }
#endif
            float smoothing = 1f - Mathf.Exp(-DeltaTime * 3.6f);
            pointerCurrent = Vector2.Lerp(pointerCurrent, pointerTarget, smoothing);

            if (settings.anomalyEnabled && settings.anomalyFrequency > 0f && anomalyRoutine == null && Clock >= nextAnomalyTime)
            {
                TriggerAnomaly();
            }

            runtimeMaterial.SetVector(ResolutionId, new Vector4(Screen.width, Screen.height, 0f, 0f));
            runtimeMaterial.SetVector(PointerId, pointerCurrent);
            runtimeMaterial.SetVector(SwirlCenterId, settings.swirlCenter);
            runtimeMaterial.SetFloat(MotionTimeId, Clock);
            runtimeMaterial.SetFloat(MasterId, settings.masterIntensity * reduction);
            runtimeMaterial.SetFloat(SpeedId, settings.motionSpeed * reduction);
            runtimeMaterial.SetFloat(SwirlId, settings.swirlStrength);
            runtimeMaterial.SetFloat(RadiusId, settings.swirlRadius);
            runtimeMaterial.SetFloat(CoreBreathId, settings.coreBreathStrength);
            runtimeMaterial.SetFloat(CoreMotionId, settings.coreMotionPixels * reduction);
            runtimeMaterial.SetFloat(InkWarpId, settings.inkWarpPixels * reduction);
            runtimeMaterial.SetFloat(LeftUiProtectWidthId, settings.leftUiProtectWidth);
            runtimeMaterial.SetFloat(EnergyId, settings.energyStrength);
            runtimeMaterial.SetFloat(ParallaxId, settings.parallaxPixels * reduction);
            runtimeMaterial.SetFloat(GrainId, settings.grainStrength);
            runtimeMaterial.SetFloat(PrintDriftId, settings.printDriftPixels * reduction);
            runtimeMaterial.SetFloat(AnomalyId, anomalyValue * reduction);
            runtimeMaterial.SetFloat(DebugMaskId, settings.debugMask ? 1f : 0f);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            Camera eventCamera = eventData.pressEventCamera != null ? eventData.pressEventCamera : ResolveEventCamera();
            TrySetPointerTarget(eventData.position, eventCamera);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // A full-screen background receives PointerExit when the cursor merely crosses onto
            // a Button above it. Preserve parallax in that case; only reset after leaving the
            // actual background rectangle/window.
            Camera eventCamera = eventData.pressEventCamera != null ? eventData.pressEventCamera : ResolveEventCamera();
            if (!TrySetPointerTarget(eventData.position, eventCamera))
            {
                pointerTarget = Vector2.zero;
            }
        }

        private Camera ResolveEventCamera()
        {
            if (targetCanvas == null) targetCanvas = targetImage != null ? targetImage.canvas : null;
            return targetCanvas == null || targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : targetCanvas.worldCamera;
        }

        private bool TrySetPointerTarget(Vector2 screenPosition, Camera eventCamera)
        {
            if (!HallMotionRuntimeGuards.IsFinite(screenPosition)) return false;
            RectTransform rect = transform as RectTransform;
            if (rect == null
                || !HallMotionRuntimeGuards.HasUsableScale(rect)
                || !RectTransformUtility.RectangleContainsScreenPoint(rect, screenPosition, eventCamera)
                || !RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPosition, eventCamera, out Vector2 local))
            {
                return false;
            }

            Rect bounds = rect.rect;
            pointerTarget = new Vector2(
                Mathf.Clamp((local.x - bounds.center.x) / Mathf.Max(0.5f, bounds.width * 0.5f), -1f, 1f),
                Mathf.Clamp((local.y - bounds.center.y) / Mathf.Max(0.5f, bounds.height * 0.5f), -1f, 1f));
            return HallMotionRuntimeGuards.IsFinite(pointerTarget);
        }

        public void TriggerAnomaly()
        {
            if (anomalyRoutine == null && isActiveAndEnabled)
            {
                anomalyRoutine = StartCoroutine(PlayAnomaly());
            }
        }

        public void ApplyJson(TextAsset jsonConfig)
        {
            if (jsonConfig == null)
            {
                return;
            }

            settings.ApplyJson(jsonConfig.text);
            ScheduleNextAnomaly();
        }

        private IEnumerator PlayAnomaly()
        {
            float startedAt = Clock;
            float duration = Mathf.Max(0.1f, settings.anomalyDuration);
            while (Clock - startedAt < duration)
            {
                float progress = Mathf.Clamp01((Clock - startedAt) / duration);
                anomalyValue = Mathf.Sin(progress * Mathf.PI) * (0.72f + 0.28f * Mathf.Sin(progress * Mathf.PI * 5f));
                yield return null;
            }

            anomalyValue = 0f;
            anomalyRoutine = null;
            ScheduleNextAnomaly();
        }

        private void ScheduleNextAnomaly()
        {
            float frequency = Mathf.Max(0.01f, settings.anomalyFrequency);
            float min = Mathf.Lerp(settings.anomalyIntervalAtMinFrequency.x, settings.anomalyIntervalAtMaxFrequency.x, frequency);
            float max = Mathf.Lerp(settings.anomalyIntervalAtMinFrequency.y, settings.anomalyIntervalAtMaxFrequency.y, frequency);
            float random01 = anomalyRandom != null ? (float)anomalyRandom.NextDouble() : 0.5f;
            nextAnomalyTime = Clock + Mathf.Lerp(min, max, random01);
        }
    }
}
