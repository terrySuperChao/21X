using System;
using UnityEngine;

namespace Miscalculation.HallMotion
{
    /// <summary>按钮的长期视觉类型；Highlighted 是策划指定的长期提示类型，不等于 EventSystem 当前选中。</summary>
    public enum MenuButtonVariant
    {
        Standard = 0,
        Highlighted = 1
    }

    [CreateAssetMenu(fileName = "MenuInteractionSettings", menuName = "Miscalculation/Menu Interaction Settings")]
    public sealed class MenuInteractionSettings : ScriptableObject
    {
        [Header("Hover / focus")]
        [Tooltip("真实 Button 命中框在常态文字左右各增加的留白像素。它不改变霓虹线的左右延伸。")]
        [Range(0f, 32f)] public float hitPaddingX = 12f;
        [Tooltip("鼠标悬停或键盘/手柄聚焦时的文字缩放倍率，只作用于 LabelVisualRoot。")]
        [Range(1f, 1.35f)] public float hoverTextScale = 1.12f;
        [Tooltip("悬停文字黑色描边的目标像素值；运行时会按 TMP 字号换算为 outlineWidth。")]
        [Range(0f, 5f)] public float hoverOutlineWidthPx = 2.2f;
        [Tooltip("进入悬停/焦点状态的过渡时间，使用 Unscaled Time，不受暂停影响。")]
        [Range(0.04f, 0.26f)] public float hoverEnterSeconds = 0.11f;
        [Tooltip("离开悬停/焦点状态的过渡与霓虹淡出时间，使用 Unscaled Time。")]
        [Range(0.06f, 0.32f)] public float hoverExitSeconds = 0.15f;

        [Header("Press / confirm")]
        [Tooltip("按住时 InteractionVisualRoot 的整体缩放倍率。不要把该根节点绑定到 Button 自身。")]
        [Range(0.92f, 1f)] public float pressScale = 0.98f;
        [Tooltip("松开确认后黑白线条收束到统一中心、文字回弹的总时长。立即切场景的业务回调应改接完成事件。")]
        [Range(0.08f, 0.32f)] public float confirmDurationSeconds = 0.16f;

        [Header("Settled scribble")]
        [Tooltip("霓虹绘制完成后的呼吸幅度。1.0 的峰值约为基础辉光两倍；0 可关闭持续呼吸并减少网格刷新。")]
        [Range(0f, 1.2f)] public float glowBreathAmount = 0.07f;
        [Tooltip("完成态霓虹辉光一次呼吸所需秒数。只影响辉光，不改变线芯颜色和几何路径。")]
        [Range(0.7f, 3.2f)] public float glowBreathPeriodSeconds = 1.7f;

        [Header("Highlighted variant prompt")]
        [Tooltip("长期高亮按钮两次偶发错位提示之间的最短随机间隔。")]
        [Range(1.2f, 8f)] public float highlightPromptMinSeconds = 2.5f;
        [Tooltip("长期高亮按钮两次偶发错位提示之间的最长随机间隔，不能小于最短间隔。")]
        [Range(1.8f, 12f)] public float highlightPromptMaxSeconds = 5f;
        [Tooltip("偶发提示达到峰值时，青紫文字回声相对正文的最大错位像素。")]
        [Range(0f, 7f)] public float highlightPromptOffset = 3f;
        [Tooltip("一次青紫错位提示从出现到恢复的总时长。")]
        [Range(0.08f, 1f)] public float highlightPromptDurationSeconds = 0.36f;
        [Tooltip("长期高亮按钮的青色文字回声颜色。")]
        public Color highlightCyan = new Color32(37, 238, 240, 255);
        [Tooltip("长期高亮按钮的紫色文字回声颜色。")]
        public Color highlightMagenta = new Color32(193, 37, 231, 255);
        [Tooltip("悬停/焦点状态的 TMP 文字描边颜色；标准预设为接近黑色。")]
        public Color hoverOutlineColor = new Color32(5, 4, 7, 255);

        public void ApplyWebJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            MenuInteractionRootJson root = JsonUtility.FromJson<MenuInteractionRootJson>(json);
            if (root == null || root.menuInteraction == null)
            {
                return;
            }

            MenuInteractionJson data = root.menuInteraction;
            hitPaddingX = data.hitPaddingX;
            hoverTextScale = data.hoverTextScale;
            hoverOutlineWidthPx = data.hoverOutlineWidth;
            hoverEnterSeconds = data.hoverEnterMs * 0.001f;
            hoverExitSeconds = data.hoverExitMs * 0.001f;
            pressScale = data.pressScale;
            confirmDurationSeconds = data.confirmDurationMs * 0.001f;
            glowBreathAmount = data.glowBreathAmount;
            glowBreathPeriodSeconds = data.glowBreathPeriodMs * 0.001f;
            highlightPromptMinSeconds = data.highlightPromptMinMs * 0.001f;
            highlightPromptMaxSeconds = data.highlightPromptMaxMs * 0.001f;
            highlightPromptOffset = data.highlightPromptOffset;
            highlightPromptDurationSeconds = data.highlightPromptDurationMs > 0f
                ? data.highlightPromptDurationMs * 0.001f
                : highlightPromptDurationSeconds;
            ClampValues();
        }

        private void OnValidate()
        {
            ClampValues();
        }

        private void ClampValues()
        {
            hitPaddingX = Mathf.Clamp(hitPaddingX, 0f, 32f);
            hoverTextScale = Mathf.Clamp(hoverTextScale, 1f, 1.35f);
            hoverOutlineWidthPx = Mathf.Clamp(hoverOutlineWidthPx, 0f, 5f);
            hoverEnterSeconds = Mathf.Clamp(hoverEnterSeconds, 0.04f, 0.26f);
            hoverExitSeconds = Mathf.Clamp(hoverExitSeconds, 0.06f, 0.32f);
            pressScale = Mathf.Clamp(pressScale, 0.92f, 1f);
            confirmDurationSeconds = Mathf.Clamp(confirmDurationSeconds, 0.08f, 0.32f);
            glowBreathAmount = Mathf.Clamp(glowBreathAmount, 0f, 1.2f);
            glowBreathPeriodSeconds = Mathf.Clamp(glowBreathPeriodSeconds, 0.7f, 3.2f);
            highlightPromptMinSeconds = Mathf.Clamp(highlightPromptMinSeconds, 1.2f, 8f);
            highlightPromptMaxSeconds = Mathf.Clamp(highlightPromptMaxSeconds, Mathf.Max(1.8f, highlightPromptMinSeconds), 12f);
            highlightPromptOffset = Mathf.Clamp(highlightPromptOffset, 0f, 7f);
            highlightPromptDurationSeconds = Mathf.Clamp(highlightPromptDurationSeconds, 0.08f, 1f);
        }
    }

    [Serializable]
    public sealed class MenuInteractionRootJson
    {
        public MenuInteractionJson menuInteraction;
    }

    [Serializable]
    public sealed class MenuInteractionJson
    {
        public float hitPaddingX;
        public float hoverTextScale;
        public float hoverOutlineWidth;
        public float hoverEnterMs;
        public float hoverExitMs;
        public float pressScale;
        public float confirmDurationMs;
        public float glowBreathAmount;
        public float glowBreathPeriodMs;
        public float highlightPromptMinMs;
        public float highlightPromptMaxMs;
        public float highlightPromptOffset;
        public float highlightPromptDurationMs;
        public string layoutMode;
        public string stateModel;
    }
}
