using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Miscalculation.HallMotion.Editor
{
    /// <summary>
    /// 主界面动效接入预检。
    ///
    /// 它不修改场景，只检查最容易让示例正常、正式工程异常的绑定：TMP、真实 Button
    /// 命中 Graphic、视觉根、Canvas 缩放、删除线目标、LayoutGroup 和 EventSystem。
    /// 程序同事导入包后应先打开目标主菜单场景，再运行本菜单项。
    /// </summary>
    public static class HallMotionIntegrationValidator
    {
        public sealed class Result
        {
            public readonly List<string> errors = new List<string>();
            public readonly List<string> warnings = new List<string>();
            public bool Passed => errors.Count == 0;
        }

        [MenuItem("Tools/Miscalculation/Hall Motion/Validate Open Scenes")]
        public static void ValidateOpenScenesMenu()
        {
            Result result = ValidateOpenScenes(true);
            EditorUtility.DisplayDialog(
                "Hall Motion Validation",
                result.Passed
                    ? $"通过。警告 {result.warnings.Count} 项，详情见 Console。"
                    : $"未通过。错误 {result.errors.Count} 项，警告 {result.warnings.Count} 项，详情见 Console。",
                "OK");
        }

        public static Result ValidateOpenScenes(bool logToConsole)
        {
            Result result = new Result();
            MenuScribbleHover[] adapters = Object.FindObjectsOfType<MenuScribbleHover>(true);
            for (int i = 0; i < adapters.Length; i++)
            {
                ValidateButton(adapters[i], result);
            }

            EventSystem[] eventSystems = Object.FindObjectsOfType<EventSystem>(true);
            int activeEventSystems = 0;
            for (int i = 0; i < eventSystems.Length; i++)
            {
                if (eventSystems[i].isActiveAndEnabled)
                {
                    activeEventSystems++;
                }
            }
            if (activeEventSystems > 1)
            {
                result.errors.Add($"场景中存在 {activeEventSystems} 个启用的 EventSystem；主菜单只能有一个。");
            }
            else if (activeEventSystems == 0 && adapters.Length > 0)
            {
                result.errors.Add("场景包含菜单特效按钮，但没有启用的 EventSystem。");
            }

            if (adapters.Length == 0)
            {
                result.warnings.Add("当前打开场景中没有 MenuScribbleHover，未执行按钮级检查。");
            }

            if (logToConsole)
            {
                for (int i = 0; i < result.errors.Count; i++)
                {
                    Debug.LogError("[Hall Motion Validation] " + result.errors[i]);
                }
                for (int i = 0; i < result.warnings.Count; i++)
                {
                    Debug.LogWarning("[Hall Motion Validation] " + result.warnings[i]);
                }
                if (result.Passed)
                {
                    Debug.Log($"[Hall Motion Validation] PASS — {adapters.Length} button(s), {result.warnings.Count} warning(s).");
                }
            }

            return result;
        }

        private static void ValidateButton(MenuScribbleHover adapter, Result result)
        {
            string path = GetPath(adapter.transform);
            Button button = adapter.TargetButton != null ? adapter.TargetButton : adapter.GetComponent<Button>();
            TMP_Text label = adapter.Label;
            RectTransform interactionRoot = adapter.InteractionVisualRoot;
            ProceduralNeonScribbleGraphic scribble = adapter.Scribble;
            ProceduralDisabledSlashGraphic slash = adapter.DisabledSlash;

            if (button == null)
            {
                result.errors.Add($"{path}: 缺少业务 Button。");
                return;
            }
            if (label == null)
            {
                result.errors.Add($"{path}: Label 未绑定 TMP_Text。");
            }
            else if (string.IsNullOrEmpty(label.text))
            {
                result.warnings.Add($"{path}: TMP Label 当前为空；运行时会保留原 Button 命中框并暂时隐藏依赖文字范围的特效。");
            }

            RectTransform buttonRect = button.transform as RectTransform;
            if (interactionRoot == null)
            {
                result.errors.Add($"{path}: InteractionVisualRoot 未绑定。");
            }
            else
            {
                if (interactionRoot == buttonRect)
                {
                    result.errors.Add($"{path}: InteractionVisualRoot 不能是 Button 自身，否则按压缩放会改变命中区域。");
                }
                if (!interactionRoot.IsChildOf(button.transform))
                {
                    result.errors.Add($"{path}: InteractionVisualRoot 必须是 Button 子级。");
                }
            }

            if (adapter.LabelVisualRoot == null)
            {
                result.errors.Add($"{path}: LabelVisualRoot 未绑定。");
            }
            if (scribble == null)
            {
                result.errors.Add($"{path}: ProceduralNeonScribbleGraphic 未绑定。");
            }
            else
            {
                if (scribble.raycastTarget)
                {
                    result.errors.Add($"{path}: Scribble Raycast Target 必须关闭。");
                }
                if (scribble.TextTarget == null)
                {
                    result.errors.Add($"{path}: Scribble Text Target 未绑定。");
                }
                else if (label != null && scribble.TextTarget != label)
                {
                    result.errors.Add($"{path}: Scribble Text Target 与按钮 Label 不是同一个 TMP。");
                }
                if (scribble.Settings == null || scribble.InteractionSettings == null)
                {
                    result.errors.Add($"{path}: Scribble Settings 或 Interaction Settings 未绑定。");
                }
            }

            if (slash == null)
            {
                result.warnings.Add($"{path}: DisabledSlash 未显式绑定；运行时会自动创建，但正式 Prefab 建议保存明确引用。");
            }
            else
            {
                if (slash.raycastTarget)
                {
                    result.errors.Add($"{path}: DisabledSlash Raycast Target 必须关闭。");
                }
                if (slash.TextTarget == null && slash.TextBoundsFallback == null)
                {
                    result.errors.Add($"{path}: DisabledSlash 同时缺少 Text Target 和 Text Bounds Fallback。");
                }
                else if (label != null && slash.TextTarget != null && slash.TextTarget != label)
                {
                    result.errors.Add($"{path}: DisabledSlash Text Target 与按钮 Label 不一致。");
                }
            }

            Graphic hitGraphic = button.targetGraphic;
            if (hitGraphic == null || hitGraphic.transform != button.transform || !hitGraphic.raycastTarget)
            {
                result.errors.Add($"{path}: Button.targetGraphic 必须是 Button 根节点上启用 Raycast Target 的固定 Graphic。");
            }
            if (label != null && label.raycastTarget)
            {
                result.errors.Add($"{path}: TMP Label Raycast Target 必须关闭，不能让文字或霓虹扩大命中区。");
            }

            Transform cursor = adapter.transform;
            while (cursor != null)
            {
                Vector3 scale = cursor.lossyScale;
                if (!IsFinite(scale)
                    || Mathf.Abs(scale.x) <= 0.00001f
                    || Mathf.Abs(scale.y) <= 0.00001f
                    || Mathf.Abs(scale.z) <= 0.00001f)
                {
                    result.errors.Add($"{path}: {cursor.name} 的世界缩放包含 0/NaN/Infinity；Canvas 和所有视觉根通常应保持 (1,1,1)。");
                    break;
                }
                if (cursor.GetComponent<Canvas>() != null)
                {
                    break;
                }
                cursor = cursor.parent;
            }

            Canvas canvas = adapter.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                result.errors.Add($"{path}: 不在 Canvas 下。");
            }
            else
            {
                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                if (scaler == null)
                {
                    result.warnings.Add($"{path}: Canvas 没有 CanvasScaler；当前主界面基准为 1920×1080 Scale With Screen Size。");
                }
                else if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize
                    || Vector2.Distance(scaler.referenceResolution, new Vector2(1920f, 1080f)) > 0.1f)
                {
                    result.warnings.Add($"{path}: CanvasScaler 不是 1920×1080 Scale With Screen Size，需重新核对定位和性能。");
                }
            }

            LayoutGroup parentLayout = adapter.transform.parent != null
                ? adapter.transform.parent.GetComponent<LayoutGroup>()
                : null;
            if (adapter.AutoSizesHitboxToText && parentLayout != null && adapter.GetComponent<LayoutElement>() == null)
            {
                result.warnings.Add($"{path}: 父级存在 {parentLayout.GetType().Name} 且按钮没有 LayoutElement；自动文字宽度可能被布局系统覆盖。");
            }
        }

        private static bool IsFinite(Vector3 value)
        {
            return IsFinite(value.x) && IsFinite(value.y) && IsFinite(value.z);
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private static string GetPath(Transform target)
        {
            string path = target.name;
            while (target.parent != null)
            {
                target = target.parent;
                path = target.name + "/" + path;
            }
            return path;
        }
    }
}
