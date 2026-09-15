using System;
using UnityEngine;

namespace Miscalculation.CharacterLobby
{
    /// <summary>所有距离使用 1920×1080 美术坐标；不存窗口坐标或粒子瞬时状态。</summary>
    [Serializable]
    public sealed class LobbyParameters
    {
        public float smokeDensity = .92f, smokeRise = 1, smokeVisibility = 1, smokeStrandWidth = 1;
        public bool smokeEnabled = true;
        public float emberIntensity = 1, emberSize = 1, sparkIntensity = 1;
        public float rainDensity = .72f, rainSpeed = .48f;
        public float rainMaskTopLeftX = 144, rainMaskTopRightX = 552, rainMaskBottomX = 193, rainMaskBottomY = 143;
        public bool rainMaskDebug;
        public float dustDensity = .72f, dustBrightness = 1.15f, dustSize = 1.1f, lampTransition = 420;
        public float sceneLampStrength = 1, sceneFarBrightness = .48f, sceneOffBrightness = .32f, sceneMoonStrength = .46f;
        public bool rainEnabled = true, dustEnabled = true, reducedMotion, lampOn = true;
        public uint seed = 0x20260820;
        public LobbyParameters Clone() { return (LobbyParameters)MemberwiseClone(); }

        public void Validate()
        {
            foreach (LobbyParameterSpec spec in LobbyParameterSpec.All)
            {
                float v = spec.Get(this);
                if (!LobbyMath.Finite(v) || v < spec.Min || v > spec.Max)
                    throw new ArgumentException(spec.Key + " 超出范围 " + spec.Min + "—" + spec.Max);
            }
            if (rainMaskTopRightX - rainMaskTopLeftX < 20 || rainMaskBottomY < 1)
                throw new ArgumentException("雨区三角形退化");
            if (seed == 0) throw new ArgumentException("随机种子不能为 0");
        }
    }

    /// <summary>运行时始终克隆 Parameters；不把 Play 模式调参写回出厂资产。</summary>
    [CreateAssetMenu(menuName = "Miscalculation/Character Lobby/Settings")]
    public sealed class LobbySettings : ScriptableObject
    {
        public const string Version = "1.0.8";
        public const string Algorithm = "lobby-unity-v1-rain-visual-occlusion-lightfield-xorshift32-flipbook24";
        public LobbyParameters parameters = new LobbyParameters();

        // Unity 对已存在 ScriptableObject 新增序列化字段时会把 float 留为 0，
        // 因此在升级旧 Settings 资产时只补齐这个不可能是合法值的缺省状态。
        void OnEnable()
        {
            if (parameters == null) parameters = new LobbyParameters();
            if (parameters.sceneLampStrength <= 0) parameters.sceneLampStrength = 1;
            if (parameters.sceneFarBrightness <= 0) parameters.sceneFarBrightness = .48f;
            if (parameters.sceneOffBrightness <= 0) parameters.sceneOffBrightness = .32f;
            if (parameters.sceneMoonStrength <= 0) parameters.sceneMoonStrength = .46f;
        }
    }

    public sealed class LobbyParameterSpec
    {
        public readonly string Key, Label, Description, Unit;
        public readonly float Min, Max, Step;
        readonly System.Reflection.FieldInfo field;
        LobbyParameterSpec(string key, string label, float min, float max, float step, string unit, string description)
        { Key = key; Label = label; Min = min; Max = max; Step = step; Unit = unit; Description = description; field = typeof(LobbyParameters).GetField(key); }
        public float Get(LobbyParameters p) { return (float)field.GetValue(p); }
        public void Set(LobbyParameters p, float value) { field.SetValue(p, value); }
        // 仅控制台交互/导入时反射，不在动效帧循环中读取。
        public static readonly LobbyParameterSpec[] All = {
            new LobbyParameterSpec("smokeDensity", "烟雾浓度", .25f, 1.6f, .01f, "倍", "调整烟带整体显色，不增加烟层和绘制次数；保留青紫笔触。与可见度相乘后上限为 1。"),
            new LobbyParameterSpec("smokeRise", "烟雾流速", .35f, 1.55f, .05f, "倍", "控制序列帧推进速度，1 倍为 4.8 帧/秒，24 帧约 5 秒循环；相邻帧持续交叉混合。根部不移动。"),
            new LobbyParameterSpec("smokeVisibility", "烟雾可见度", .25f, 2.2f, .05f, "倍", "提高或降低青紫烟雾的可见程度，不产生大面积雾团。屏幕混合，浓度与可见度存在饱和上限。"),
            new LobbyParameterSpec("smokeStrandWidth", "烟带宽度", .45f, 1.8f, .05f, "倍", "仅以烟头为锚点横向缩放；高度不变。1 倍显示 282×456 个设计像素。"),
            new LobbyParameterSpec("emberIntensity", "烟头余烬强度", 0, 2.5f, .05f, "倍", "正在燃烧香烟的持续火点亮度，0 为关闭。低频呼吸会随台灯亮度调整反差，不影响碎火星。"),
            new LobbyParameterSpec("emberSize", "烟头余烬大小", .4f, 2.4f, .05f, "倍", "以 (1564,399) 为中心缩放烟头亮芯与局部辉光，不移动烟雾发射点。"),
            new LobbyParameterSpec("sparkIntensity", "碎火星与散火点", 0, 2.5f, .05f, "倍", "控制烟头偶发碎火星和烟灰缸 7 处散落火点的亮度。碎火星固定池上限 14，减少动态时不发射。"),
            new LobbyParameterSpec("rainDensity", "窗口雨量", .15f, 1.45f, .01f, "倍", "控制可见雨滴数，最多 168；使用三角形面积采样与分散顺序，雨速独立调节。"),
            new LobbyParameterSpec("rainSpeed", "雨滴速度", .08f, 1.25f, .02f, "倍", "控制下落速度，默认 0.48；较小值更慢，不改变雨量。上下区域有轻微可见度补偿。"),
            new LobbyParameterSpec("dustDensity", "微尘数量", 0, 1.5f, .01f, "倍", "仅台灯点亮后可见，最多 140 粒；为保留旧版观感，1.25 起达到数量上限，继续调大不增加粒子。"),
            new LobbyParameterSpec("dustBrightness", "微尘亮度", .2f, 3, .05f, "倍", "提高灯下暖色微尘亮度。与灯光强度联动；关灯不可见，可配合微尘大小辨认。"),
            new LobbyParameterSpec("dustSize", "微尘大小", .5f, 2.5f, .05f, "倍", "微尘亮芯与柔边同比例缩放。无全屏模糊，数值大时更容易看清。"),
            new LobbyParameterSpec("rainMaskTopLeftX", "雨区 A · 左上 X", -160, 260, 1, "px", "左上顶点横坐标，Y 固定为 0。允许负值移出美术左边缘；所有分辨率共用设计坐标。"),
            new LobbyParameterSpec("rainMaskTopRightX", "雨区 B · 右上 X", 360, 720, 1, "px", "右上顶点横坐标，Y 固定为 0。打开雨区边界可同时查看三角形和灯具轮廓遮挡；大厅标题不再禁雨。"),
            new LobbyParameterSpec("rainMaskBottomX", "雨区 C · 下端 X", -160, 360, 1, "px", "三角形下端横坐标。允许为负数，不随窗口分辨率漂移。"),
            new LobbyParameterSpec("rainMaskBottomY", "雨区 C · 下端 Y", 70, 260, 1, "px", "三角形下端纵坐标，从美术上边缘向下计量；越大越向桌面延伸，请用调试边界精调。"),
            new LobbyParameterSpec("lampTransition", "台灯切换时长", 180, 800, 20, "ms", "台灯明暗和色温过渡用时；不改变角色资料。连续切换从当前亮度平滑反向，不闪回起点。"),
            new LobbyParameterSpec("sceneLampStrength", "物件台灯强度", 0, 1.8f, .05f, "倍", "控制场景物件对低分辨率灯光场 R 通道的响应。只影响挂有 LobbySceneItem 的场景元素；纯 UI 不受影响。"),
            new LobbyParameterSpec("sceneFarBrightness", "开灯远处亮度", .18f, .9f, .02f, "倍", "开灯状态下远离台灯的场景物件基础亮度。近处会在此基础上按灯光场渐变增强。"),
            new LobbyParameterSpec("sceneOffBrightness", "关灯环境亮度", .08f, .62f, .02f, "倍", "关灯时所有场景物件的统一环境亮度基线；窗外月光会在此基础上少量叠加。"),
            new LobbyParameterSpec("sceneMoonStrength", "物件月光强度", 0, 1.1f, .05f, "倍", "控制场景物件对灯光场 G 通道的冷色月光响应。不会启用实时灯、阴影或法线计算。")
        };
    }
}
