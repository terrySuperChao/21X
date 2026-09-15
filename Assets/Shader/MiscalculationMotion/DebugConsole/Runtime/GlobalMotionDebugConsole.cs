using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Miscalculation.Motion.Card;
using Miscalculation.Motion.Common;
using Miscalculation.Motion.LevelSelect;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Miscalculation.Motion.DebugConsole
{
    public enum GlobalMotionPage { Common, LevelDeal, BattleDeal, LevelInteraction }
    public enum GlobalCommonDemo { ItemMemory, CardArtwork, DifficultySelection, ChapterSelection, StampDrop, ButtonFeedback, FullScreen }
    public enum GlobalMotionCommand
    {
        ItemAppear, ItemDisappear, CardArtworkAppear, CardArtworkDisappear, CardArtworkRebuild,
        DifficultyCircle, ChapterCircle, StampDrop, ButtonA, ButtonB, ButtonC,
        ScreenEntrance, ScreenExit, ScreenReset, DealLevel, DealBattle, ResetCurrent
    }

    /// <summary>可选运行时调参模块；所有验证控件均位于独立 Debug Canvas，删除本目录不影响功能源码。</summary>
    [DisallowMultipleComponent]
    public sealed class GlobalMotionDebugConsole : MonoBehaviour
    {
        public const string Version = "1.0.6";
        public TMP_FontAsset font;
        public KeyCode toggleKey = KeyCode.F10;
        public bool visible = true;
        public CommonMotionProfile common = new CommonMotionProfile();
        public CardMotionProfile levelDeal = CreateLevelDeal();
        public CardMotionProfile battleDeal = new CardMotionProfile();
        public LevelSelectMotionProfile levelInteraction = new LevelSelectMotionProfile();

        public GlobalMotionPage Page { get; private set; }
        public GlobalCommonDemo CommonDemo { get; private set; }
        public bool Visible => built && panel && panel.gameObject.activeSelf;
        public bool Dirty { get; private set; }
        public string LastStatus { get; private set; }
        public string DifficultySampleText => difficultySampleText;
        public float ChapterSampleSize => chapterSampleSize;

        public event Action ProfilesChanged;
        public event Action ViewChanged;
        public event Action<GlobalMotionCommand> CommandRequested;
        public event Action<string> DifficultyTextChanged;
        public event Action<float> ChapterSampleSizeChanged;

        sealed class NumericSpec
        {
            public string label, description, unit; public float min, max, step; public Func<float> get; public Action<float> set; public bool profileValue = true;
        }
        sealed class Binding { public NumericSpec spec; public Slider slider; public TMP_InputField input; }

        RectTransform panel, tooltip, content, modal;
        ScrollRect parameterScroll;
        GameObject launcher;
        TextMeshProUGUI statusText, statsText, tooltipText, sectionTitle;
        TMP_InputField presetName, difficultyText, difficultyColorInput, chapterColorInput;
        Toggle reduceMotionToggle;
        Button firstFocus;
        Action confirmed;
        GameObject previousFocus;
        readonly List<Binding> bindings = new List<Binding>();
        readonly List<Selectable> navigation = new List<Selectable>();
        readonly string[] lastFiles = new string[4];
        CommonMotionProfile beforeCommon;
        CardMotionProfile beforeCard;
        LevelSelectMotionProfile beforeLevel;
        bool built, sync, moduleEnabled = true;
        string difficultySampleText = "地狱";
        float chapterSampleSize = 72f;
        float statsElapsed, statsTotal;
        int statsFrames;

        static readonly Color Ink = new Color(.075f, .085f, .115f, .985f);
        static readonly Color Paper = new Color(.9f, .88f, .83f);
        static readonly Color Accent = new Color(.3f, .86f, .85f);
        static readonly Color Field = new Color(.12f, .14f, .18f);
        static readonly string[] DemoLabels = { "物品显隐", "牌面重构", "难度画圈", "章节节点", "盖章落下", "按钮反馈", "全屏过渡" };

        void Start()
        {
            if (!font) font = TMP_Settings.defaultFontAsset;
            Build(); built = true; BuildParameterContent(); RefreshValues(); SetVisible(visible);
            SetStatus("调参只影响当前运行；导出 JSON 后才会保存。F10 隐藏/展开。", false);
        }

        public void SetVisible(bool value)
        {
            visible = value;
            if (!built || !moduleEnabled) return;
            ShowTooltip(null, null); panel.gameObject.SetActive(value); launcher.SetActive(!value);
            statsElapsed = statsTotal = 0; statsFrames = 0;
            if (EventSystem.current)
            {
                if (value) { previousFocus = EventSystem.current.currentSelectedGameObject; EventSystem.current.SetSelectedGameObject(firstFocus ? firstFocus.gameObject : null); }
                else EventSystem.current.SetSelectedGameObject(launcher);
            }
        }

        public void SetModuleEnabled(bool value)
        {
            moduleEnabled = value;
            if (panel) panel.gameObject.SetActive(value && visible);
            if (launcher) launcher.SetActive(value && !visible);
            enabled = value;
        }

        public void SelectView(GlobalMotionPage page, GlobalCommonDemo demo = GlobalCommonDemo.ItemMemory)
        {
            bool changed = Page != page || (page == GlobalMotionPage.Common && CommonDemo != demo);
            Page = page; if (page == GlobalMotionPage.Common) CommonDemo = demo;
            if (!built || !changed) return;
            BuildParameterContent(); RefreshValues(); ViewChanged?.Invoke();
        }

        RectTransform Rect(string name, Transform parent, float x, float y, float w, float h)
        {
            var value = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            value.SetParent(parent, false); value.anchorMin = value.anchorMax = value.pivot = new Vector2(0, 1);
            value.anchoredPosition = new Vector2(x, -y); value.sizeDelta = new Vector2(w, h); return value;
        }
        Image Box(RectTransform rect, Color color, bool raycast = true) { var image = rect.gameObject.AddComponent<Image>(); image.color = color; image.raycastTarget = raycast; return image; }
        TextMeshProUGUI Text(Transform parent, string value, float x, float y, float w, float h, float size = 18)
        {
            RectTransform rect = Rect(value, parent, x, y, w, h); var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = font; text.fontSize = size; text.color = Paper; text.text = value; text.raycastTarget = false;
            text.enableWordWrapping = true; text.overflowMode = TextOverflowModes.Overflow; return text;
        }
        Button MakeButton(Transform parent, string label, float x, float y, float w, float h, Action action, string tip = null)
        {
            RectTransform rect = Rect(label, parent, x, y, w, h); Image image = Box(rect, Field); var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image; var colors = button.colors; colors.highlightedColor = new Color(.26f, .34f, .42f); colors.selectedColor = colors.highlightedColor; colors.pressedColor = new Color(.18f, .55f, .56f); button.colors = colors;
            TextMeshProUGUI text = Text(rect, label, 5, 3, w - 10, h - 6, 17); text.alignment = TextAlignmentOptions.Center;
            button.onClick.AddListener(() => action()); navigation.Add(button); if (!string.IsNullOrEmpty(tip)) AddTip(rect, tip); return button;
        }
        TMP_InputField MakeInput(Transform parent, string name, float x, float y, float w, float h)
        {
            RectTransform rect = Rect(name, parent, x, y, w, h); Image image = Box(rect, Field); var input = rect.gameObject.AddComponent<TMP_InputField>(); input.targetGraphic = image;
            RectTransform viewport = Rect("Text Area", rect, 7, 2, w - 14, h - 4); viewport.gameObject.AddComponent<RectMask2D>();
            TextMeshProUGUI text = Text(viewport, "", 0, 0, w - 14, h - 4, 17); text.alignment = TextAlignmentOptions.MidlineLeft; text.enableWordWrapping = false;
            input.textViewport = viewport; input.textComponent = text; input.fontAsset = font; input.customCaretColor = true; input.caretColor = Accent; input.characterLimit = 120; navigation.Add(input); return input;
        }
        Toggle MakeToggle(Transform parent, string label, float x, float y, float w, Action<bool> action, string description)
        {
            RectTransform rect = Rect(label, parent, x, y, w, 32); Image hit = Box(rect, new Color(0, 0, 0, 0)); var toggle = rect.gameObject.AddComponent<Toggle>(); toggle.targetGraphic = hit;
            RectTransform square = Rect("Box", rect, 0, 4, 22, 22); Box(square, Field); RectTransform check = Rect("Check", square, 4, 4, 14, 14); toggle.graphic = Box(check, Accent, false);
            Text(rect, label, 30, 2, w - 30, 30, 17); toggle.onValueChanged.AddListener(v => { if (!sync) action(v); }); AddTip(rect, description); navigation.Add(toggle); return toggle;
        }
        void AddTip(RectTransform rect, string description)
        {
            var target = rect.gameObject.AddComponent<GlobalMotionTooltipTarget>(); target.owner = this; target.description = description;
            if (!rect.GetComponent<Graphic>()) Box(rect, new Color(0, 0, 0, 0));
        }

        void Build()
        {
            panel = Rect("Global Motion Parameter Console", transform, 0, 18, 500, 1044); panel.anchorMin = panel.anchorMax = new Vector2(1, 1); panel.pivot = new Vector2(1, 1); panel.anchoredPosition = new Vector2(-18, -18); Box(panel, Ink);
            RectTransform header = Rect("Drag Header", panel, 0, 0, 500, 54); Box(header, new Color(.11f, .13f, .17f)); var drag = header.gameObject.AddComponent<GlobalMotionConsoleDrag>(); drag.target = panel;
            Text(header, "全局通用动效 · v" + Version, 18, 10, 390, 36, 21); MakeButton(header, "收起", 424, 10, 64, 34, () => SetVisible(false));
            Text(panel, "Unity 原生验证 / 1920×1080 / 游戏切图分层", 18, 62, 464, 26, 16).color = Accent;

            string[] tabs = { "通用", "关卡发牌", "战斗发牌", "关卡交互" };
            for (int i = 0; i < tabs.Length; i++)
            {
                int index = i; Button button = MakeButton(panel, tabs[i], 18 + i * 116, 96, 108, 38, () => SelectView((GlobalMotionPage)index));
                if (i == 0) firstFocus = button;
            }
            reduceMotionToggle = MakeToggle(panel, "减少动态", 18, 142, 160, v => { common.reducedMotion = v; Changed(); }, "缩短全屏过渡并关闭非必要的强回弹；不改变最终状态和按钮命中区域。");
            sectionTitle = Text(panel, "", 190, 142, 292, 32, 17); sectionTitle.alignment = TextAlignmentOptions.MidlineRight; sectionTitle.color = Accent;

            RectTransform scrollRect = Rect("Parameters Scroll", panel, 14, 184, 472, 624); Box(scrollRect, new Color(.04f, .05f, .07f, .86f)); parameterScroll = scrollRect.gameObject.AddComponent<ScrollRect>();
            RectTransform viewport = Rect("Viewport", scrollRect, 6, 4, 444, 616); Box(viewport, new Color(0, 0, 0, 0)); viewport.gameObject.AddComponent<RectMask2D>();
            content = Rect("Content", viewport, 0, 0, 436, 800); parameterScroll.viewport = viewport; parameterScroll.content = content; parameterScroll.horizontal = false; parameterScroll.vertical = true; parameterScroll.movementType = ScrollRect.MovementType.Clamped; parameterScroll.scrollSensitivity = 34;
            RectTransform barRect = Rect("Scrollbar", scrollRect, 454, 4, 12, 616); Image barImage = Box(barRect, new Color(.08f, .1f, .14f)); var scrollbar = barRect.gameObject.AddComponent<Scrollbar>(); scrollbar.direction = Scrollbar.Direction.BottomToTop;
            RectTransform handleArea = Rect("Sliding Area", barRect, 2, 2, 8, 612); RectTransform handle = Rect("Handle", handleArea, 0, 0, 8, 80); handle.anchorMin = new Vector2(0, 0); handle.anchorMax = new Vector2(1, .18f); handle.offsetMin = handle.offsetMax = Vector2.zero; scrollbar.handleRect = handle; scrollbar.targetGraphic = Box(handle, Accent);
            parameterScroll.verticalScrollbar = scrollbar; parameterScroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

            Text(panel, "预设名称", 18, 820, 104, 30, 17); presetName = MakeInput(panel, "Preset Name", 122, 820, 360, 32); presetName.SetTextWithoutNotify("全局通用动效参数"); presetName.onEndEdit.AddListener(_ => { Dirty = true; UpdateStatus(); });
            MakeButton(panel, "导入 JSON", 18, 862, 144, 38, ImportDialog, "选择当前大分类对应的 JSON。兼容同一 Schema 的旧版本；失败时不修改现有参数。");
            MakeButton(panel, "导出 JSON", 178, 862, 144, 38, ExportDialog, "另存当前大分类的参数 JSON，并记住该路径。");
            MakeButton(panel, "保存当前文件", 338, 862, 144, 38, SaveCurrent, "覆盖最近一次导入或导出的当前大分类 JSON；没有路径时转为另存。");
            MakeButton(panel, "撤销导入", 18, 910, 144, 34, UndoImport, "恢复本次运行中最近一次导入前的参数。");
            MakeButton(panel, "恢复默认", 178, 910, 144, 34, () => Confirm("恢复当前分类的出厂参数？\n未保存调参将丢失。", RestoreDefaults), "只恢复当前大分类，不影响其他分类参数。");
            statsText = Text(panel, "", 338, 910, 144, 34, 15); statsText.alignment = TextAlignmentOptions.MidlineRight;
            statusText = Text(panel, "", 18, 952, 464, 58, 15); statusText.overflowMode = TextOverflowModes.Ellipsis;
            Text(panel, "F10 显示/隐藏；功能源码与验证场景、控制台物理分离。", 18, 1012, 464, 24, 14).color = Accent;

            launcher = MakeButton(transform, "展开控制台 · F10", 0, 18, 210, 40, () => SetVisible(true)).gameObject;
            RectTransform launcherRect = launcher.transform as RectTransform; launcherRect.anchorMin = launcherRect.anchorMax = launcherRect.pivot = new Vector2(1, 1); launcherRect.anchoredPosition = new Vector2(-18, -18); launcher.SetActive(false);
            tooltip = Rect("Single Tooltip", transform, 0, 0, 438, 184); Box(tooltip, new Color(.035f, .045f, .065f, .995f), false); tooltipText = Text(tooltip, "", 16, 12, 406, 160, 17); tooltip.gameObject.SetActive(false);
            modal = Rect("Confirm", panel, 0, 0, 500, 1044); Box(modal, new Color(0, 0, 0, .76f)); RectTransform dialog = Rect("Dialog", modal, 28, 378, 444, 218); Box(dialog, new Color(.06f, .08f, .11f, 1)); Text(dialog, "", 18, 18, 408, 120, 19);
            MakeButton(dialog, "确认", 30, 160, 174, 40, () => { Action action = confirmed; CloseModal(); action?.Invoke(); }); MakeButton(dialog, "取消", 240, 160, 174, 40, CloseModal); modal.gameObject.SetActive(false);
        }

        void BuildParameterContent()
        {
            bindings.Clear(); difficultyText = difficultyColorInput = chapterColorInput = null;
            navigation.RemoveAll(value => !value || value.transform.IsChildOf(content));
            for (int i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject);
            float y = 10;
            if (Page == GlobalMotionPage.Common)
            {
                for (int i = 0; i < DemoLabels.Length; i++)
                {
                    int index = i; float x = (i % 2) * 214 + 6; float row = i / 2;
                    MakeButton(content, (i == (int)CommonDemo ? "● " : "") + DemoLabels[i], x, y + row * 40, 202, 34, () => SelectView(GlobalMotionPage.Common, (GlobalCommonDemo)index));
                }
                y += 166; y = BuildCommonActions(y);
                foreach (NumericSpec spec in CommonSpecs()) y = AddNumericRow(spec, y);
            }
            else if (Page == GlobalMotionPage.LevelDeal || Page == GlobalMotionPage.BattleDeal)
            {
                bool level = Page == GlobalMotionPage.LevelDeal;
                CardMotionProfile profile = level ? levelDeal : battleDeal;
                Toggle flightFlip = MakeToggle(content, "飞行中翻到正面", 6, y, 420,
                    value => { profile.flip.mode = value ? "duringFlight" : "afterLand"; Changed(); },
                    "开启后，牌在飞行途中完成翻面并以正面落地；关闭后先以牌背落地，再原地翻面。关卡与战斗参数分别保存。");
                sync = true; flightFlip.isOn = profile.flip.mode == "duringFlight"; sync = false; y += 42;
                MakeButton(content, level ? "播放关卡发牌" : "播放战斗发牌", 6, y, 202, 36, () => Request(level ? GlobalMotionCommand.DealLevel : GlobalMotionCommand.DealBattle), "使用当前分类独立参数，从场景提供的牌堆锚点发到目标槽位；JSON 不保存任何屏幕坐标。");
                MakeButton(content, "重置牌位", 224, y, 202, 36, () => Request(GlobalMotionCommand.ResetCurrent), "停止当前序列并把本页所有牌恢复到初始隐藏状态。"); y += 52;
                foreach (NumericSpec spec in DealSpecs(profile)) y = AddNumericRow(spec, y);
            }
            else
            {
                MakeButton(content, "重置关卡交互", 6, y, 420, 36, () => Request(GlobalMotionCommand.ResetCurrent), "把手牌和公共牌恢复到初始位置，取消当前组合并还原中央图案。"); y += 52;
                foreach (NumericSpec spec in LevelInteractionSpecs()) y = AddNumericRow(spec, y);
            }
            content.sizeDelta = new Vector2(436, Mathf.Max(616, y + 12)); content.anchoredPosition = Vector2.zero;
            parameterScroll.verticalNormalizedPosition = 1; sectionTitle.text = SectionName();
        }

        float BuildCommonActions(float y)
        {
            switch (CommonDemo)
            {
                case GlobalCommonDemo.ItemMemory:
                    MakeButton(content, "物品出现", 6, y, 202, 36, () => Request(GlobalMotionCommand.ItemAppear), "只让大厅工具和牌堆在原位凝聚出现，与全屏入场没有任何自动关系。");
                    MakeButton(content, "物品消失", 224, y, 202, 36, () => Request(GlobalMotionCommand.ItemDisappear), "只让大厅工具和牌堆在原位剥落消失，与全屏离场没有任何自动关系。"); break;
                case GlobalCommonDemo.CardArtwork:
                    MakeButton(content, "中央图案出现", 6, y, 132, 36, () => Request(GlobalMotionCommand.CardArtworkAppear)); MakeButton(content, "中央图案消失", 148, y, 132, 36, () => Request(GlobalMotionCommand.CardArtworkDisappear)); MakeButton(content, "旧图重构为新图", 290, y, 136, 36, () => Request(GlobalMotionCommand.CardArtworkRebuild)); break;
                case GlobalCommonDemo.DifficultySelection:
                    Text(content, "直接点击任一难度按钮播放画圈", 6, y + 3, 202, 32, 15).color = Accent;
                    difficultyText = MakeInput(content, "Difficulty Sample Text", 224, y, 202, 36); difficultyText.SetTextWithoutNotify(difficultySampleText); difficultyText.onEndEdit.AddListener(v => { difficultySampleText = string.IsNullOrWhiteSpace(v) ? "地狱" : v.Trim(); difficultyText.SetTextWithoutNotify(difficultySampleText); DifficultyTextChanged?.Invoke(difficultySampleText); }); AddTip(difficultyText.transform as RectTransform, "只修改验证场景的最后一个难度示例文字与按钮宽度，用于测试多语言长度自适应；不会修改正式游戏按钮。");
                    difficultyColorInput = AddColorRow("难度画圈颜色", common.difficultySelection, y + 48, "独立控制难度按钮圆圈与箭头的实色笔芯；输入六位 RGB 色值，例如 #440006。透明度固定为 100%，避免不同底图导致额外色差。"); return y + 100;
                case GlobalCommonDemo.ChapterSelection:
                    Text(content, "直接点击任一章节节点播放画圈", 6, y + 3, 420, 32, 15).color = Accent;
                    chapterColorInput = AddColorRow("章节画圈颜色", common.chapterSelection, y + 48, "独立控制全部章节节点圆圈的实色笔芯；输入六位 RGB 色值，例如 #7B0611。所有节点使用同一不透明色，柔边处仍会受背景明暗的视觉对比影响。"); return y + 100;
                case GlobalCommonDemo.StampDrop: MakeButton(content, "播放盖章落下", 6, y, 420, 36, () => Request(GlobalMotionCommand.StampDrop), "图片基本锁定落点中心，通过近镜头大比例和桌面阴影从淡散到收紧，模拟垂直于桌面从空间上方砸落；不沿桌面方向滑行。"); break;
                case GlobalCommonDemo.ButtonFeedback:
                    Text(content, "直接点击场景中的 A / B / C 按钮查看各自反馈", 6, y + 3, 420, 32, 15).color = Accent; break;
                case GlobalCommonDemo.FullScreen:
                    MakeButton(content, "黑场显露", 6, y, 132, 36, () => Request(GlobalMotionCommand.ScreenEntrance)); MakeButton(content, "逐渐黑场", 148, y, 132, 36, () => Request(GlobalMotionCommand.ScreenExit)); MakeButton(content, "重置为可见", 290, y, 136, 36, () => Request(GlobalMotionCommand.ScreenReset)); break;
            }
            return y + 52;
        }

        TMP_InputField AddColorRow(string label, SelectionMotionSettings settings, float y, string description)
        {
            TextMeshProUGUI title = Text(content, label, 6, y + 3, 150, 30, 17); title.raycastTarget = true; AddTip(title.rectTransform, description);
            TMP_InputField input = MakeInput(content, label + " Value", 160, y, 194, 34); input.characterLimit = 7; input.SetTextWithoutNotify(settings.color);
            RectTransform swatchRect = Rect(label + " Swatch", content, 368, y, 56, 34); Image swatch = Box(swatchRect, ParseOpaqueColor(settings.color, Color.white), false); AddTip(input.transform as RectTransform, description);
            input.onEndEdit.AddListener(raw =>
            {
                if (!TryNormalizeOpaqueHex(raw, out string normalized)) { input.SetTextWithoutNotify(settings.color); SetStatus(label + "：请输入 #RRGGBB 六位色值。", true); return; }
                settings.color = normalized; input.SetTextWithoutNotify(normalized); swatch.color = ParseOpaqueColor(normalized, Color.white); Changed();
            });
            return input;
        }

        static bool TryNormalizeOpaqueHex(string raw, out string normalized)
        {
            normalized = null; if (string.IsNullOrWhiteSpace(raw)) return false; string value = raw.Trim(); if (value[0] != '#') value = "#" + value;
            if (value.Length != 7 || !ColorUtility.TryParseHtmlString(value, out _)) return false; normalized = value.ToUpperInvariant(); return true;
        }
        static Color ParseOpaqueColor(string value, Color fallback) { if (!ColorUtility.TryParseHtmlString(value, out Color color)) color = fallback; color.a = 1; return color; }

        float AddNumericRow(NumericSpec spec, float y)
        {
            TextMeshProUGUI label = Text(content, spec.label, 6, y, 282, 28, 17); label.raycastTarget = true;
            AddTip(label.rectTransform, spec.description + "\n范围：" + spec.min.ToString("0.###") + "—" + spec.max.ToString("0.###") + " " + spec.unit + "；步长：" + spec.step.ToString("0.###") + "。");
            TMP_InputField input = MakeInput(content, spec.label + " Value", 310, y, 114, 30); input.characterLimit = 14;
            RectTransform track = Rect("Slider " + spec.label, content, 10, y + 38, 410, 24); Box(track, new Color(.08f, .1f, .14f)); var slider = track.gameObject.AddComponent<Slider>();
            RectTransform fillArea = Rect("Fill Area", track, 6, 8, 398, 8); RectTransform fill = Rect("Fill", fillArea, 0, 0, 398, 8); fill.anchorMin = Vector2.zero; fill.anchorMax = Vector2.one; fill.offsetMin = fill.offsetMax = Vector2.zero; Box(fill, Accent, false); slider.fillRect = fill;
            RectTransform handleArea = Rect("Handle Area", track, 6, 0, 398, 24); RectTransform handle = Rect("Handle", handleArea, 0, 0, 16, 0); handle.pivot = new Vector2(.5f, .5f); slider.targetGraphic = Box(handle, Paper); slider.handleRect = handle; slider.minValue = spec.min; slider.maxValue = spec.max; navigation.Add(slider);
            var binding = new Binding { spec = spec, slider = slider, input = input }; bindings.Add(binding);
            slider.onValueChanged.AddListener(v => { if (sync) return; float q = Mathf.Round(v / spec.step) * spec.step; spec.set(Mathf.Clamp(q, spec.min, spec.max)); ApplySpecChange(spec); });
            input.onEndEdit.AddListener(v => { if (sync) return; if (float.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out float n) && MotionMath.Finite(n) && n >= spec.min && n <= spec.max) { spec.set(n); ApplySpecChange(spec); } else { SetStatus(spec.label + "：请输入范围内的数字。", true); RefreshValues(); } });
            return y + 74;
        }

        void ApplySpecChange(NumericSpec spec)
        {
            if (spec.profileValue) Changed();
            else { RefreshValues(); SetStatus(spec.label + "仅用于验证场景，未写入参数 JSON。", false); }
        }

        IEnumerable<NumericSpec> CommonSpecs()
        {
            if (CommonDemo == GlobalCommonDemo.ItemMemory) return MemorySpecs(common.itemMemory, "物品");
            if (CommonDemo == GlobalCommonDemo.CardArtwork) return MemorySpecs(common.cardArtworkMemory, "中央图案", true);
            if (CommonDemo == GlobalCommonDemo.DifficultySelection) return SelectionSpecs(common.difficultySelection, "难度按钮");
            if (CommonDemo == GlobalCommonDemo.ChapterSelection)
            {
                var list = new List<NumericSpec>(SelectionSpecs(common.chapterSelection, "章节节点"))
                {
                    Spec("测试节点尺寸", "只改变验证场景中的一个章节节点，用于确认圆圈会随目标图片大小实时适配；不写入正式参数 JSON。", 28, 160, 1, "px", () => chapterSampleSize, v => { chapterSampleSize = v; ChapterSampleSizeChanged?.Invoke(v); }, false)
                };
                return list;
            }
            if (CommonDemo == GlobalCommonDemo.StampDrop) return StampSpecs();
            if (CommonDemo == GlobalCommonDemo.ButtonFeedback) return ButtonSpecs();
            return ScreenSpecs();
        }
        IEnumerable<NumericSpec> MemorySpecs(MemoryTransitionSettings p, string subject, bool includeRebuild = false)
        {
            var list = new List<NumericSpec>
            {
                Spec(subject + "出现时长", "记忆碎片在原位凝聚并完整显现的总用时。", 220, 1600, 20, "ms", () => p.appearDurationMs, v => p.appearDurationMs = v),
                Spec(subject + "消失时长", "图像在原位剥落为记忆残片并完全消失的总用时。", 220, 1600, 20, "ms", () => p.disappearDurationMs, v => p.disappearDurationMs = v),
                Spec("记忆碎片尺寸", "控制记忆岛的整体尺度；越大越偏大块，不增加节点或 Draw Call。", .55f, 2.4f, .05f, "倍", () => p.fragmentScale, v => p.fragmentScale = v),
                Spec("碎片边缘柔度", "控制碎片边缘过渡宽度；较小更利落，较大更像褪色记忆。", .015f, .18f, .005f, "倍", () => p.fragmentSoftness, v => p.fragmentSoftness = v),
                Spec("碎片离散程度", "控制大块记忆岛和细小断点的混合比例，没有统一扫掠方向。", 0, 1, .02f, "倍", () => p.fragmentIrregularity, v => p.fragmentIrregularity = v),
                Spec("记忆残影强度", "控制轮廓内部短暂的炭黑暗紫残影，不产生外发光或飞散节点。", 0, .6f, .02f, "倍", () => p.fringeStrength, v => p.fringeStrength = v)
            };
            if (includeRebuild) list.Insert(2, Spec("图案重构总时长", "旧中央图案消失后，新中央图案再出现的完整时长。", 160, 1200, 20, "ms", () => p.rebuildDurationMs, v => p.rebuildDurationMs = v));
            return list;
        }
        IEnumerable<NumericSpec> SelectionSpecs(SelectionMotionSettings p, string subject)
        {
            return new[]
            {
                Spec(subject + "画线时长", "只控制" + subject + "的画线总时长；难度先画圈再画箭头，章节只画圈。", 260, 1600, 20, "ms", () => p.durationMs, v => p.durationMs = v),
                Spec(subject + "画线宽度", "只控制" + subject + "的手绘主笔画宽度，不改变目标命中区域。", 1, 12, .2f, "px", () => p.strokeWidth, v => p.strokeWidth = v),
                Spec(subject + "画线柔边", "只控制" + subject + "线条边缘的透明羽化宽度，减少锯齿但不使用模糊。", .25f, 4, .05f, "px", () => p.edgeFeather, v => p.edgeFeather = v),
                Spec(subject + "收尖长度", "只控制" + subject + "圆圈首尾的收尖；难度按钮外置箭头不受影响。", 4, 34, 1, "px", () => p.taperLength, v => p.taperLength = v),
                Spec(subject + "横向留白", "按目标文字或图标宽度向左右扩展圆圈，适配多语言或不同节点尺寸。", 4, 60, 1, "px", () => p.paddingX, v => p.paddingX = v),
                Spec(subject + "纵向留白", "按目标高度向上下扩展圆圈，不修改目标自身尺寸。", 2, 40, 1, "px", () => p.paddingY, v => p.paddingY = v)
            };
        }
        IEnumerable<NumericSpec> StampSpecs()
        {
            ImageTransferSettings p = common.transfer;
            return new[]
            {
                Spec("盖章总时长", "垂直于桌面从空间上方落下、接触压缩到回弹复位的总时间。", 160, 1200, 20, "ms", () => p.durationMs, v => p.durationMs = v),
                Spec("高度投影纵移", "物体离开桌面后因斜视镜头产生的少量屏幕纵向投影；0 表示始终锁定落点中心，不代表沿桌面移动。", -180, 180, 2, "px", () => p.heightProjectionY, v => p.heightProjectionY = v),
                Spec("高空起始比例", "用近大远小模拟物体处于镜头与桌面之间的高处；落地时回到 1 倍。", 1f, 5f, .05f, "倍", () => p.startScale, v => p.startScale = v),
                Spec("高空阴影尺度", "物体尚高时桌面投影较散的尺寸；下降时逐渐收紧到落点。", .8f, 2.5f, .02f, "倍", () => p.shadowStartScale, v => p.shadowStartScale = v),
                Spec("高空阴影强度", "物体尚高时桌面投影的透明度，宜保持很淡。", 0, .5f, .01f, "Alpha", () => p.shadowStartAlpha, v => p.shadowStartAlpha = v),
                Spec("接触阴影强度", "物体贴近桌面及落稳后的接触阴影透明度。", 0, .65f, .01f, "Alpha", () => p.shadowContactAlpha, v => p.shadowContactAlpha = v),
                Spec("撞击横向压缩", "接触桌面瞬间的 X 轴缩放，略大于 1 会产生横向挤压。", .75f, 1.35f, .01f, "倍", () => p.impactScaleX, v => p.impactScaleX = v),
                Spec("撞击纵向压缩", "接触桌面瞬间的 Y 轴缩放，小于 1 会形成盖章冲击。", .65f, 1.2f, .01f, "倍", () => p.impactScaleY, v => p.impactScaleY = v),
                Spec("落地回弹", "撞击后回到正常比例时的轻微回弹幅度。", 0, .25f, .005f, "倍", () => p.rebound, v => p.rebound = v),
                Spec("起始倾角", "下落开始时的轻微倾斜，接触时归零。", -20, 20, .5f, "度", () => p.rotationDeg, v => p.rotationDeg = v),
                Spec("撞击停顿", "接触后保持压缩姿态的短暂停顿，用于增强重量感。", 0, 180, 5, "ms", () => p.impactHoldMs, v => p.impactHoldMs = v)
            };
        }
        IEnumerable<NumericSpec> ButtonSpecs()
        {
            var list = new List<NumericSpec>(); AddButton(list, common.buttonA, "A · 纯文字"); AddButton(list, common.buttonB, "B · 方框"); AddButton(list, common.buttonC, "C · 图标"); return list;
        }
        void AddButton(List<NumericSpec> list, ButtonPressSettings p, string prefix)
        {
            list.Add(Spec(prefix + "时间", "按下到回弹复位的总时长；只移动视觉子层，不改变 Button 根命中框。", 60, 800, 10, "ms", () => p.durationMs, v => p.durationMs = v));
            list.Add(Spec(prefix + "横向缩放", "按压峰值的 X 轴比例。", .72f, 1.2f, .01f, "倍", () => p.pressScaleX, v => p.pressScaleX = v));
            list.Add(Spec(prefix + "纵向缩放", "按压峰值的 Y 轴比例。", .72f, 1.2f, .01f, "倍", () => p.pressScaleY, v => p.pressScaleY = v));
            list.Add(Spec(prefix + "下压位移", "按压峰值的垂直位移，负值向下。", -30, 30, 1, "px", () => p.offsetY, v => p.offsetY = v));
            list.Add(Spec(prefix + "倾斜角", "按压过程的短暂旋转；图标按钮可略强，文字按钮宜接近 0。", -12, 12, .2f, "度", () => p.rotationDeg, v => p.rotationDeg = v));
            list.Add(Spec(prefix + "回弹", "返回静止状态前的轻微超调幅度。", 0, .25f, .005f, "倍", () => p.overshoot, v => p.overshoot = v));
        }
        IEnumerable<NumericSpec> ScreenSpecs()
        {
            FullScreenTransitionSettings p = common.screen;
            return new[]
            {
                Spec("入场显露时长", "从完整黑场平滑显露当前界面的时间。", 80, 1800, 20, "ms", () => p.entranceDurationMs, v => p.entranceDurationMs = v),
                Spec("离场转黑时长", "从当前界面平滑过渡到完整黑场的时间。", 80, 1800, 20, "ms", () => p.exitDurationMs, v => p.exitDurationMs = v),
                Spec("入场黑场停留", "开始显露前保持完整黑场的时间。", 0, 800, 20, "ms", () => p.entranceBlackHoldMs, v => p.entranceBlackHoldMs = v),
                Spec("离场黑场停留", "完全变黑后，到完成事件发出前继续保持的时间。", 0, 800, 20, "ms", () => p.exitBlackHoldMs, v => p.exitBlackHoldMs = v),
                Spec("减少动态时长", "开启减少动态后，入场和离场统一使用的短过渡时间。", 40, 600, 20, "ms", () => p.reducedDurationMs, v => p.reducedDurationMs = v)
            };
        }
        IEnumerable<NumericSpec> DealSpecs(CardMotionProfile p)
        {
            return new[]
            {
                Spec("发牌预备时间", "牌从牌堆抽离前的短暂蓄势时间。", 0, 450, 5, "ms", () => p.draw.anticipationMs, v => p.draw.anticipationMs = v),
                Spec("抽牌抬起距离", "牌离开牌堆时沿视觉纵向抬起的设计像素。", 0, 90, 1, "px", () => p.draw.liftPx, v => p.draw.liftPx = v),
                Spec("飞行时间", "单张牌从牌堆到目标槽位的飞行时间。", 160, 1200, 5, "ms", () => p.flight.durationMs, v => p.flight.durationMs = v),
                Spec("飞行弧高", "牌的抛物线最高点高度，不保存起点和目标坐标。", 0, 360, 2, "px", () => p.flight.arcPx, v => p.flight.arcPx = v),
                Spec("飞行横向偏移", "给轨迹增加受控横向偏移；正负方向由参数决定。", -140, 140, 1, "px", () => p.flight.lateralPx, v => p.flight.lateralPx = v),
                Spec("起始旋转", "牌离开牌堆时的初始角度。", 0, 35, .5f, "度", () => p.flight.startRotationDeg, v => p.flight.startRotationDeg = v),
                Spec("飞行旋转量", "整个飞行阶段附加的旋转量。", -90, 90, 1, "度", () => p.flight.spinDeg, v => p.flight.spinDeg = v),
                Spec("飞行起始缩放", "牌离开牌堆时相对最终尺寸的比例。", .7f, 1.15f, .01f, "倍", () => p.flight.startScale, v => p.flight.startScale = v),
                Spec("落地时间", "飞行结束后压缩、滑移和回弹复位的时间。", 70, 700, 5, "ms", () => p.land.durationMs, v => p.land.durationMs = v),
                Spec("落地横向压缩", "牌接触桌面时的 X 轴比例。", .72f, 1.05f, .005f, "倍", () => p.land.squashX, v => p.land.squashX = v),
                Spec("落地纵向拉伸", "牌接触桌面时的 Y 轴比例。", .95f, 1.28f, .005f, "倍", () => p.land.squashY, v => p.land.squashY = v),
                Spec("落地滑移", "牌接触桌面后沿原飞行方向继续滑向最终牌位的距离；现在与空中飞行明确分段，并使用摩擦减速。", 0, 140, 1, "px", () => p.land.slidePx, v => p.land.slidePx = v),
                Spec("接触停顿", "牌压到桌面后的短暂停顿；数值越大，接触与后续摩擦滑行的分段越清楚。", 0, 180, 5, "ms", () => p.land.impactHoldMs, v => p.land.impactHoldMs = v),
                Spec("横向散落", "每张牌目标位置的确定性横向微偏移上限。", 0, 55, 1, "px", () => p.land.scatterXPx, v => p.land.scatterXPx = v),
                Spec("纵向散落", "每张牌目标位置的确定性纵向微偏移上限。", 0, 35, 1, "px", () => p.land.scatterYPx, v => p.land.scatterYPx = v),
                Spec("落地角度随机", "每张牌落地后的确定性角度微差上限。", 0, 12, .2f, "度", () => p.land.rotationJitterDeg, v => p.land.rotationJitterDeg = v),
                Spec("翻牌时间", "牌背收窄、切换正面并展开的完整时间。", 90, 900, 5, "ms", () => p.flip.durationMs, v => p.flip.durationMs = v),
                Spec("翻牌中点停顿", "牌最窄处保持的短暂停顿，强化翻面辨识。", 0, 260, 5, "ms", () => p.flip.midpointHoldMs, v => p.flip.midpointHoldMs = v),
                Spec("飞行翻牌起点", "开启飞行中翻牌后，单张牌飞行进度达到该比例时开始翻面；数值越小越早开始。", .25f, .82f, .01f, "比例", () => p.flip.flightStart, v => p.flip.flightStart = v),
                Spec("连续发牌间隔", "同一发牌组中相邻两张牌开始动作的时间差。", 0, 600, 5, "ms", () => p.sequence.dealGapMs, v => p.sequence.dealGapMs = v)
            };
        }
        IEnumerable<NumericSpec> LevelInteractionSpecs()
        {
            LevelSelectMotionProfile p = levelInteraction;
            return new[]
            {
                Spec("拖牌抬起倍率", "拖动时只放大视觉牌面，命中判断仍使用原业务区域。", 1, 1.3f, .01f, "倍", () => p.dragScale, v => p.dragScale = v),
                Spec("拖牌最大倾角", "水平拖动越快，牌越向运动反方向倾斜；这里限制最大角度。缓慢竖直拖动会接近 0 度，停止移动时会自动回正，不影响落点判定。", 0, 15, .5f, "度", () => p.dragTiltMaxDeg, v => p.dragTiltMaxDeg = v),
                Spec("达到满倾角拖速", "水平拖动速度达到该值时使用最大倾角；数值越小越容易看到倾斜，按 Canvas 设计像素每秒计算。", 120, 2400, 20, "px/s", () => p.dragTiltFullSpeedPxPerSec, v => p.dragTiltFullSpeedPxPerSec = v),
                Spec("有效吸附时间", "拖到有效公共牌后吸附到组合槽位的时间。", 80, 800, 10, "ms", () => p.snapDurationMs, v => p.snapDurationMs = v),
                Spec("无效返回时间", "拖到无效区域后沿短弧返回原手牌位置的时间。", 80, 1000, 10, "ms", () => p.returnDurationMs, v => p.returnDurationMs = v),
                Spec("无效返回弧高", "无效返回轨迹的最大抬升高度。", 0, 260, 2, "px", () => p.returnArcPx, v => p.returnArcPx = v),
                Spec("组合落桌力度", "吸附成功后，手牌和公共牌围绕共同中心同步压小、短促回弹并恢复。1 为关闭；1.035 表示压小 3.5%，1.15 表示压小 15%。旧 JSON 的目标确认脉冲数值继续有效。", 1, 1.15f, .005f, "倍", () => p.validPulseScale, v => p.validPulseScale = v),
                Spec("组合落桌时长", "两张牌同步落桌反馈的总时间，包含快速下压、短暂停留、一次小幅回弹和停稳。默认 180 毫秒；不会阻塞继续拖牌或点击撤回。", 80, 600, 10, "ms", () => p.validPulseDurationMs, v => p.validPulseDurationMs = v),
                Spec("图案重建延迟", "吸附确认后等待多久开始中央图案重构。", 0, 400, 5, "ms", () => p.rebuildDelayMs, v => p.rebuildDelayMs = v)
            };
        }
        static NumericSpec Spec(string label, string description, float min, float max, float step, string unit, Func<float> get, Action<float> set, bool profileValue = true) => new NumericSpec { label = label, description = description, min = min, max = max, step = step, unit = unit, get = get, set = set, profileValue = profileValue };

        string SectionName()
        {
            if (Page == GlobalMotionPage.Common) return "通用 / " + DemoLabels[(int)CommonDemo];
            if (Page == GlobalMotionPage.LevelDeal) return "关卡发牌参数";
            if (Page == GlobalMotionPage.BattleDeal) return "战斗发牌参数";
            return "关卡交互参数";
        }
        void Request(GlobalMotionCommand command) { CommandRequested?.Invoke(command); }
        void Changed() { Dirty = true; try { common.Validate(); levelDeal.ClampInPlace(); battleDeal.ClampInPlace(); levelInteraction.Validate(); ProfilesChanged?.Invoke(); SetStatus("参数已应用到当前运行，尚未保存。", false); } catch (Exception e) { SetStatus(e.Message, true); } RefreshValues(); }
        void RefreshValues()
        {
            if (!built) return; sync = true;
            for (int i = 0; i < bindings.Count; i++) { Binding binding = bindings[i]; float value = binding.spec.get(); binding.slider.SetValueWithoutNotify(value); if (!binding.input.isFocused) binding.input.SetTextWithoutNotify(value.ToString("0.###", CultureInfo.InvariantCulture)); }
            if (reduceMotionToggle) reduceMotionToggle.SetIsOnWithoutNotify(common.reducedMotion);
            sync = false; UpdateStatus();
        }
        void SetStatus(string message, bool error) { LastStatus = message; if (statusText) statusText.color = error ? new Color(1, .55f, .5f) : Paper; UpdateStatus(); }
        void UpdateStatus() { if (statusText) statusText.text = (Dirty ? "[未保存] " : "[已保存/默认] ") + (LastStatus ?? ""); }

        string SelectedJson()
        {
            if (Page == GlobalMotionPage.Common) return MotionProfileIO.ToJson(common);
            if (Page == GlobalMotionPage.LevelInteraction) return LevelSelectProfileIO.Export(levelInteraction);
            return CardMotionProfileIO.Export(Page == GlobalMotionPage.LevelDeal ? levelDeal : battleDeal);
        }
        string PresetDirectory { get { string path = Path.Combine(Application.persistentDataPath, "GlobalMotionPresets"); Directory.CreateDirectory(path); return path; } }
        string DefaultName() => Page + "_v" + Version + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
        public bool TryExport(string path)
        {
            try { string json = SelectedJson(); MotionProfileIO.WriteAtomic(path, json); lastFiles[(int)Page] = Path.GetFullPath(path); Dirty = false; SetStatus("已保存：" + lastFiles[(int)Page], false); return true; }
            catch (Exception e) { SetStatus("保存失败：" + e.Message, true); return false; }
        }
        public bool TryImport(string path)
        {
            try
            {
                string json = MotionProfileIO.ReadAll(path); string note;
                if (Page == GlobalMotionPage.Common) { CommonMotionProfile next = MotionProfileIO.ParseCommon(json, out note); beforeCommon = common.Clone(); common = next; }
                else if (Page == GlobalMotionPage.LevelInteraction) { LevelSelectMotionProfile next = LevelSelectProfileIO.ImportCompatible(json); beforeLevel = JsonUtility.FromJson<LevelSelectMotionProfile>(JsonUtility.ToJson(levelInteraction)); levelInteraction = next; note = "同一 Schema 兼容导入。"; }
                else { CardMotionProfile next = CardMotionProfileIO.ImportCompatible(json); beforeCard = CardMotionProfile.FromJson(JsonUtility.ToJson(Page == GlobalMotionPage.LevelDeal ? levelDeal : battleDeal)); if (Page == GlobalMotionPage.LevelDeal) levelDeal = next; else battleDeal = next; note = "同一 Schema 兼容导入。"; }
                lastFiles[(int)Page] = Path.GetFullPath(path); Dirty = false; BuildParameterContent(); RefreshValues(); ProfilesChanged?.Invoke(); SetStatus("已导入：" + Path.GetFileName(path) + "\n" + note, false); return true;
            }
            catch (Exception e) { SetStatus("导入失败，参数未改：" + e.Message, true); return false; }
        }
        void ImportDialog() { try { string path = GlobalMotionFileDialog.Choose(false, PresetDirectory, ""); if (!string.IsNullOrEmpty(path)) TryImport(path); } catch (Exception e) { SetStatus(e.Message, true); } }
        void ExportDialog() { try { string path = GlobalMotionFileDialog.Choose(true, PresetDirectory, DefaultName()); if (!string.IsNullOrEmpty(path)) TryExport(path); } catch (Exception e) { SetStatus(e.Message, true); } }
        void SaveCurrent() { string path = lastFiles[(int)Page]; if (string.IsNullOrEmpty(path)) { ExportDialog(); return; } Confirm("覆盖保存当前 JSON 文件？\n" + Path.GetFileName(path), () => TryExport(path)); }
        void UndoImport()
        {
            if (Page == GlobalMotionPage.Common && beforeCommon != null) { common = beforeCommon; beforeCommon = null; }
            else if (Page == GlobalMotionPage.LevelInteraction && beforeLevel != null) { levelInteraction = beforeLevel; beforeLevel = null; }
            else if ((Page == GlobalMotionPage.LevelDeal || Page == GlobalMotionPage.BattleDeal) && beforeCard != null) { if (Page == GlobalMotionPage.LevelDeal) levelDeal = beforeCard; else battleDeal = beforeCard; beforeCard = null; }
            else { SetStatus("当前分类暂无可撤销的导入。", false); return; }
            Dirty = true; BuildParameterContent(); RefreshValues(); ProfilesChanged?.Invoke(); SetStatus("已撤销上次导入，当前值尚未保存。", false);
        }
        void RestoreDefaults()
        {
            if (Page == GlobalMotionPage.Common) common = new CommonMotionProfile(); else if (Page == GlobalMotionPage.LevelDeal) levelDeal = CreateLevelDeal(); else if (Page == GlobalMotionPage.BattleDeal) battleDeal = new CardMotionProfile(); else levelInteraction = new LevelSelectMotionProfile();
            Dirty = true; BuildParameterContent(); RefreshValues(); ProfilesChanged?.Invoke(); ViewChanged?.Invoke(); SetStatus("已恢复当前分类默认参数，尚未保存。", false);
        }
        void Confirm(string message, Action action) { confirmed = action; modal.GetComponentInChildren<TextMeshProUGUI>().text = message; modal.gameObject.SetActive(true); modal.SetAsLastSibling(); }
        void CloseModal() { modal.gameObject.SetActive(false); confirmed = null; if (EventSystem.current && firstFocus) EventSystem.current.SetSelectedGameObject(firstFocus.gameObject); }

        public void ShowTooltip(RectTransform source, string description)
        {
            if (!tooltip) return; if (!source || string.IsNullOrEmpty(description)) { tooltip.gameObject.SetActive(false); return; }
            tooltip.gameObject.SetActive(true); tooltip.SetAsLastSibling(); tooltipText.text = description;
            RectTransform root = transform as RectTransform; Vector3 point = root.InverseTransformPoint(source.position); float left = point.x + root.rect.width * root.pivot.x - 446; float top = root.rect.height * (1 - root.pivot.y) - point.y;
            tooltip.anchoredPosition = new Vector2(Mathf.Clamp(left, 8, Mathf.Max(8, root.rect.width - 446)), -Mathf.Clamp(top, 8, Mathf.Max(8, root.rect.height - 192)));
        }
        void Update()
        {
            if (!built || !moduleEnabled) return;
#if ENABLE_LEGACY_INPUT_MANAGER
            bool typing = EventSystem.current && EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();
            if (!typing && UnityEngine.Input.GetKeyDown(toggleKey) && !modal.gameObject.activeSelf) SetVisible(!Visible);
#endif
            if (!Visible) return; statsTotal += Time.unscaledDeltaTime; statsElapsed += Time.unscaledDeltaTime; statsFrames++;
            if (statsElapsed >= .5f) { statsText.SetText("{0:0.0} FPS", statsFrames / Mathf.Max(.001f, statsTotal)); statsElapsed = statsTotal = 0; statsFrames = 0; }
        }

        static CardMotionProfile CreateLevelDeal()
        {
            CardMotionProfile value = new CardMotionProfile { profileId = "card.level-select.default", name = "关卡选牌" };
            value.flight.durationMs = 405; value.flight.arcPx = 128; value.land.durationMs = 205; value.land.slidePx = 28; value.land.scatterXPx = 5; value.land.scatterYPx = 3; value.land.rotationJitterDeg = 1.1f; value.sequence.dealGapMs = 125; value.effects.dustCount = 3; return value;
        }
    }

    public sealed class GlobalMotionTooltipTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        public GlobalMotionDebugConsole owner; public string description;
        public void OnPointerEnter(PointerEventData eventData) { owner.ShowTooltip(transform as RectTransform, description); }
        public void OnPointerExit(PointerEventData eventData) { owner.ShowTooltip(null, null); }
        public void OnSelect(BaseEventData eventData) { owner.ShowTooltip(transform as RectTransform, description); }
        public void OnDeselect(BaseEventData eventData) { owner.ShowTooltip(null, null); }
        void OnDisable() { if (owner) owner.ShowTooltip(null, null); }
    }

    public sealed class GlobalMotionConsoleDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform target; Vector2 start, position;
        public void OnBeginDrag(PointerEventData eventData) { RectTransformUtility.ScreenPointToLocalPointInRectangle(target.parent as RectTransform, eventData.position, eventData.pressEventCamera, out start); position = target.anchoredPosition; }
        public void OnDrag(PointerEventData eventData)
        {
            RectTransform parent = target.parent as RectTransform; RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, eventData.position, eventData.pressEventCamera, out Vector2 point); Vector2 value = position + point - start;
            value.x = Mathf.Clamp(value.x, -Mathf.Max(0, parent.rect.width - target.rect.width), 0); value.y = Mathf.Clamp(value.y, -Mathf.Max(0, parent.rect.height - 54), 0); target.anchoredPosition = value;
        }
        public void OnEndDrag(PointerEventData eventData) { }
    }
}
