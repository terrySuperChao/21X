using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Miscalculation.HallMotion.Editor
{
    /// <summary>
    /// 将 Web 动效实验台导出的完整 JSON 事务式写入四份 Unity Settings。
    ///
    /// 这是纯 Editor 工具，不进入 Player，也不会在运行时增加轮询、GC 或绘制开销。
    /// 导入前先在临时 ScriptableObject 上完成 Schema、版本、必需字段和有限值校验；
    /// 全部通过后才一次性覆盖正式资产，并用 Unity Undo 支持完整撤销。
    /// </summary>
    public static class HallMotionWebJsonImporter
    {
        public const string CurrentSchema = "com.miscalculation.hall-motion/v3";

        private static readonly string[] CompatibleMotionVersions =
        {
            "1.0.7",
            "1.0.8",
            "1.0.9",
            "1.0.10",
            "1.0.11",
            "1.0.12",
            HallMotionSettings.MotionVersion
        };

        private static readonly string[] RequiredBackgroundKeys =
        {
            "masterIntensity", "motionSpeed", "swirlStrength", "swirlRadius",
            "coreBreathStrength", "coreMotionPixels", "inkWarpPixels", "leftUiProtectWidth",
            "energyStrength", "parallaxPixels", "grainStrength", "printDriftPixels",
            "anomalyFrequency", "swirlCenterX", "swirlCenterY", "anomalyEnabled",
            "reducedMotion", "debugMask"
        };

        private static readonly string[] RequiredScribbleKeys =
        {
            "drawMode", "minRegionWidth", "leftOverflow", "rightOverflow", "verticalPadding",
            "regionHeight", "lineCount", "drawDurationMs", "thickness", "pressureVariation",
            "tipResidualWidth", "entryLengthRatio", "glowFadeAdvance", "dryBrushFibers",
            "splatterCount", "splatterSize", "splatterSpread", "wobble", "loopiness",
            "turnAngleDeg", "glow", "primaryColor", "secondaryColor", "accentColor",
            "sampleCount", "singleStrokePointCount", "algorithm"
        };

        private static readonly string[] RequiredInteractionKeys =
        {
            "hitPaddingX", "hoverTextScale", "hoverEnterMs", "hoverExitMs",
            "pressScale", "confirmDurationMs", "glowBreathAmount", "glowBreathPeriodMs",
            "highlightPromptMinMs", "highlightPromptMaxMs", "highlightPromptOffset",
            "highlightPromptDurationMs", "layoutMode", "stateModel"
        };

        private static readonly string[] RequiredRainKeys =
        {
            "enabled", "seed", "density", "nearRatio", "length", "speed", "angleDeg", "wind",
            "opacity", "glow", "menuProtect", "coreProtect", "gustChance", "gustStrength", "algorithm"
        };

        [Serializable]
        private sealed class WebJsonHeader
        {
            public string schema;
            public string motionVersion;
            public string renderer;
            public string aspectRatio;
            public bool reducedMotion;
            public MenuScribbleJson menuScribble;
            public MenuInteractionJson menuInteraction;
            public HallRainJson rain;
        }

        public readonly struct ImportResult
        {
            public ImportResult(bool success, string message, string sourceVersion, bool compatibilityMode, int refreshedRainLayers)
            {
                Success = success;
                Message = message;
                SourceVersion = sourceVersion;
                CompatibilityMode = compatibilityMode;
                RefreshedRainLayers = refreshedRainLayers;
            }

            public bool Success { get; }
            public string Message { get; }
            public string SourceVersion { get; }
            public bool CompatibilityMode { get; }
            public int RefreshedRainLayers { get; }
        }

        /// <summary>
        /// 一次性导入完整 Web JSON。测试可关闭 Undo、保存和场景刷新，以便只验证映射逻辑。
        /// </summary>
        public static ImportResult ImportJson(
            string json,
            HallMotionSettings motionSettings,
            HallRainSettings rainSettings,
            MenuScribbleSettings scribbleSettings,
            MenuInteractionSettings interactionSettings,
            bool recordUndo = true,
            bool saveAssets = true,
            bool refreshOpenScenes = true)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return Fail("请先停止 Play Mode，再导入网页 JSON。Settings 是持久化资产，不应在运行态修改。");
            }

            if (motionSettings == null || rainSettings == null || scribbleSettings == null || interactionSettings == null)
            {
                return Fail("四份目标 Settings 必须全部绑定：背景、雨、霓虹线和菜单交互缺一不可。");
            }

            // 通过 Package Manager 安装时，Packages/ 下的内置示例资产是只读依赖。
            // 正式导入必须明确写入项目自身 Assets/ 下的副本，避免界面看似成功、
            // 实际参数在刷新或重新解析包后丢失。测试可通过 saveAssets=false 使用临时对象。
            if (saveAssets && !TryValidateWritableProjectAssets(
                    new UnityEngine.Object[] { motionSettings, rainSettings, scribbleSettings, interactionSettings },
                    out string targetValidationError))
            {
                return Fail(targetValidationError);
            }

            if (!TryValidateEnvelope(json, out WebJsonHeader header, out string validationError))
            {
                return Fail(validationError);
            }

            HallMotionSettings motionPreview = ScriptableObject.CreateInstance<HallMotionSettings>();
            HallRainSettings rainPreview = ScriptableObject.CreateInstance<HallRainSettings>();
            MenuScribbleSettings scribblePreview = ScriptableObject.CreateInstance<MenuScribbleSettings>();
            MenuInteractionSettings interactionPreview = ScriptableObject.CreateInstance<MenuInteractionSettings>();

            try
            {
                EditorUtility.CopySerialized(motionSettings, motionPreview);
                EditorUtility.CopySerialized(rainSettings, rainPreview);
                EditorUtility.CopySerialized(scribbleSettings, scribblePreview);
                EditorUtility.CopySerialized(interactionSettings, interactionPreview);

                motionPreview.ApplyJson(json);
                rainPreview.ApplyWebJson(json);
                scribblePreview.ApplyWebJson(json);
                interactionPreview.ApplyWebJson(json);

                if (!AllImportedValuesAreFinite(motionPreview, rainPreview, scribblePreview, interactionPreview))
                {
                    return Fail("JSON 解析后出现 NaN 或 Infinity；为避免生成非法网格，本次没有写入任何资产。", header.motionVersion);
                }

                List<UnityEngine.Object> undoTargets = new List<UnityEngine.Object>
                {
                    motionSettings,
                    rainSettings,
                    scribbleSettings,
                    interactionSettings
                };

                ProceduralRainGraphic[] rainLayers = refreshOpenScenes
                    ? FindSceneObjects<ProceduralRainGraphic>().Where(layer => layer.Settings == rainSettings).ToArray()
                    : Array.Empty<ProceduralRainGraphic>();
                ProceduralNeonScribbleGraphic[] scribbles = refreshOpenScenes
                    ? FindSceneObjects<ProceduralNeonScribbleGraphic>()
                        .Where(graphic => graphic.Settings == scribbleSettings || graphic.InteractionSettings == interactionSettings)
                        .ToArray()
                    : Array.Empty<ProceduralNeonScribbleGraphic>();
                undoTargets.AddRange(rainLayers);
                undoTargets.AddRange(scribbles);
                undoTargets.AddRange(scribbles.Select(graphic => graphic.rectTransform));

                int undoGroup = -1;
                if (recordUndo)
                {
                    Undo.IncrementCurrentGroup();
                    undoGroup = Undo.GetCurrentGroup();
                    Undo.SetCurrentGroupName("Import Hall Motion Web JSON");
                    Undo.RecordObjects(undoTargets.ToArray(), "Import Hall Motion Web JSON");
                }

                try
                {
                    EditorUtility.CopySerialized(motionPreview, motionSettings);
                    EditorUtility.CopySerialized(rainPreview, rainSettings);
                    EditorUtility.CopySerialized(scribblePreview, scribbleSettings);
                    EditorUtility.CopySerialized(interactionPreview, interactionSettings);

                    MarkAssetDirty(motionSettings);
                    MarkAssetDirty(rainSettings);
                    MarkAssetDirty(scribbleSettings);
                    MarkAssetDirty(interactionSettings);

                    int refreshedRainLayers = 0;
                    if (refreshOpenScenes)
                    {
                        refreshedRainLayers = RefreshOpenScenes(
                            rainSettings,
                            scribbleSettings,
                            interactionSettings,
                            header.rain.enabled,
                            header.reducedMotion,
                            rainLayers,
                            scribbles);
                    }

                    if (saveAssets)
                    {
                        AssetDatabase.SaveAssets();
                    }

                    if (recordUndo && undoGroup >= 0)
                    {
                        Undo.CollapseUndoOperations(undoGroup);
                    }

                    bool compatibilityMode = !string.Equals(header.motionVersion, HallMotionSettings.MotionVersion, StringComparison.Ordinal);
                    string compatibilityNote = compatibilityMode
                        ? $" 已按兼容规则读取 v{header.motionVersion}；当前 Unity 工具版本为 v{HallMotionSettings.MotionVersion}。参数保留，但一笔往返会升级为上下均衡算法，不复现旧版偏上的路径；多线并发不变。"
                        : string.Empty;
                    return new ImportResult(
                        true,
                        $"已同步背景、雨、霓虹线和菜单交互四组参数。刷新雨层 {refreshedRainLayers} 个。{compatibilityNote}",
                        header.motionVersion,
                        compatibilityMode,
                        refreshedRainLayers);
                }
                catch (Exception exception)
                {
                    if (recordUndo && undoGroup >= 0)
                    {
                        Undo.RevertAllDownToGroup(undoGroup);
                    }

                    return Fail("写入阶段失败，所有修改均已回滚：" + exception.Message, header.motionVersion);
                }
            }
            catch (Exception exception)
            {
                return Fail("JSON 预检失败，尚未修改正式资产：" + exception.Message, header != null ? header.motionVersion : null);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(motionPreview);
                UnityEngine.Object.DestroyImmediate(rainPreview);
                UnityEngine.Object.DestroyImmediate(scribblePreview);
                UnityEngine.Object.DestroyImmediate(interactionPreview);
            }
        }

        private static bool TryValidateWritableProjectAssets(UnityEngine.Object[] targets, out string error)
        {
            foreach (UnityEngine.Object target in targets)
            {
                string assetPath = AssetDatabase.GetAssetPath(target).Replace('\\', '/');
                if (!AssetDatabase.Contains(target) || string.IsNullOrWhiteSpace(assetPath))
                {
                    error = $"{target.name} 不是已保存的项目资产。请先把四份 Settings 保存到 Assets/ 下，再执行导入。";
                    return false;
                }

                if (!assetPath.StartsWith("Assets/", StringComparison.Ordinal))
                {
                    error = $"{target.name} 位于只读路径 {assetPath}。请将 Package/Sample 中的四份 Settings 复制到项目 Assets/ 下，并重新绑定后再导入。";
                    return false;
                }
            }

            error = null;
            return true;
        }

        private static bool TryValidateEnvelope(string json, out WebJsonHeader header, out string error)
        {
            header = null;
            error = null;
            if (string.IsNullOrWhiteSpace(json))
            {
                error = "JSON 文件为空。";
                return false;
            }

            try
            {
                header = JsonUtility.FromJson<WebJsonHeader>(json);
            }
            catch (Exception exception)
            {
                error = "JSON 语法无法解析：" + exception.Message;
                return false;
            }

            if (header == null)
            {
                error = "JSON 根对象无法解析。";
                return false;
            }

            if (!string.Equals(header.schema, CurrentSchema, StringComparison.Ordinal))
            {
                error = $"不支持的 Schema：{header.schema ?? "<缺失>"}。当前只接受 {CurrentSchema}。";
                return false;
            }

            if (string.IsNullOrEmpty(header.motionVersion)
                || !CompatibleMotionVersions.Contains(header.motionVersion, StringComparer.Ordinal))
            {
                error = $"不支持的主界面动效版本：{header.motionVersion ?? "<缺失>"}。已知兼容版本：{string.Join(", ", CompatibleMotionVersions)}。";
                return false;
            }

            if (!string.Equals(header.aspectRatio, "16:9", StringComparison.Ordinal))
            {
                error = $"JSON 宽高比为 {header.aspectRatio ?? "<缺失>"}；当前验证工程只接受 16:9。";
                return false;
            }

            if (header.menuScribble == null || header.menuInteraction == null || header.rain == null)
            {
                error = "JSON 必须同时包含 menuScribble、menuInteraction 和 rain 三个完整对象。";
                return false;
            }

            if (!HasAllProperties(json, RequiredBackgroundKeys, out string missingBackground))
            {
                error = "背景参数不完整，缺少字段：" + missingBackground;
                return false;
            }

            string missingScribble = null;
            if (!TryExtractObject(json, "menuScribble", out string scribbleJson)
                || !HasAllProperties(scribbleJson, RequiredScribbleKeys, out missingScribble))
            {
                error = "menuScribble 参数不完整，缺少字段：" + (missingScribble ?? "对象无法读取");
                return false;
            }

            string missingInteraction = null;
            if (!TryExtractObject(json, "menuInteraction", out string interactionJson)
                || !HasAllProperties(interactionJson, RequiredInteractionKeys, out missingInteraction))
            {
                error = "menuInteraction 参数不完整，缺少字段：" + (missingInteraction ?? "对象无法读取");
                return false;
            }

            string missingRain = null;
            if (!TryExtractObject(json, "rain", out string rainJson)
                || !HasAllProperties(rainJson, RequiredRainKeys, out missingRain))
            {
                error = "rain 参数不完整，缺少字段：" + (missingRain ?? "对象无法读取");
                return false;
            }

            // Old known presets migrate PARAMETERS explicitly (see result note),
            // not old random paths. New exports must carry the new algorithm ID.
            string expectedScribbleAlgorithm = string.Equals(header.motionVersion, HallMotionSettings.MotionVersion, StringComparison.Ordinal)
                ? MenuScribbleSettings.AlgorithmVersion : "xorshift32-neon-scribble-v10-soft-lift-fibers";
            if (!string.Equals(header.menuScribble.algorithm, expectedScribbleAlgorithm, StringComparison.Ordinal))
            {
                error = "霓虹线算法版本不匹配，已拒绝导入以避免 Web/Unity 路径表现漂移。";
                return false;
            }

            // v1.0.10 introduced monotonic gust travel. Later menu-only revisions
            // keep the same v2 rain path. Older compatible
            // presets still require v1 so a version label cannot silently change random results.
            bool usesMonotonicGust = string.Equals(header.motionVersion, HallMotionSettings.MotionVersion, StringComparison.Ordinal)
                || string.Equals(header.motionVersion, "1.0.10", StringComparison.Ordinal)
                || string.Equals(header.motionVersion, "1.0.11", StringComparison.Ordinal)
                || string.Equals(header.motionVersion, "1.0.12", StringComparison.Ordinal);
            string expectedRainAlgorithm = usesMonotonicGust
                ? HallRainSettings.AlgorithmVersion
                : "xorshift32-dual-rain-v1";
            if (!string.Equals(header.rain.algorithm, expectedRainAlgorithm, StringComparison.Ordinal))
            {
                error = "雨夜算法版本不匹配，已拒绝导入以避免随机结果漂移。";
                return false;
            }

            return true;
        }

        private static bool HasAllProperties(string json, IEnumerable<string> keys, out string missing)
        {
            foreach (string key in keys)
            {
                if (!HasProperty(json, key))
                {
                    missing = key;
                    return false;
                }
            }

            missing = null;
            return true;
        }

        private static bool HasProperty(string json, string key)
        {
            string token = "\"" + key + "\"";
            int searchIndex = 0;
            while (searchIndex < json.Length)
            {
                int keyIndex = json.IndexOf(token, searchIndex, StringComparison.Ordinal);
                if (keyIndex < 0)
                {
                    return false;
                }

                int cursor = keyIndex + token.Length;
                while (cursor < json.Length && char.IsWhiteSpace(json[cursor])) cursor++;
                if (cursor < json.Length && json[cursor] == ':')
                {
                    return true;
                }

                searchIndex = cursor;
            }

            return false;
        }

        private static bool TryExtractObject(string json, string propertyName, out string objectJson)
        {
            objectJson = null;
            string token = "\"" + propertyName + "\"";
            int propertyIndex = json.IndexOf(token, StringComparison.Ordinal);
            if (propertyIndex < 0) return false;

            int colonIndex = json.IndexOf(':', propertyIndex + token.Length);
            if (colonIndex < 0) return false;

            int start = colonIndex + 1;
            while (start < json.Length && char.IsWhiteSpace(json[start])) start++;
            if (start >= json.Length || json[start] != '{') return false;

            int depth = 0;
            bool inString = false;
            bool escaped = false;
            for (int index = start; index < json.Length; index++)
            {
                char character = json[index];
                if (inString)
                {
                    if (escaped)
                    {
                        escaped = false;
                    }
                    else if (character == '\\')
                    {
                        escaped = true;
                    }
                    else if (character == '"')
                    {
                        inString = false;
                    }
                    continue;
                }

                if (character == '"')
                {
                    inString = true;
                }
                else if (character == '{')
                {
                    depth++;
                }
                else if (character == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        objectJson = json.Substring(start, index - start + 1);
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool AllImportedValuesAreFinite(
            HallMotionSettings motion,
            HallRainSettings rain,
            MenuScribbleSettings scribble,
            MenuInteractionSettings interaction)
        {
            return Finite(
                    motion.masterIntensity, motion.motionSpeed, motion.swirlStrength, motion.swirlRadius,
                    motion.coreBreathStrength, motion.coreMotionPixels, motion.inkWarpPixels,
                    motion.leftUiProtectWidth, motion.energyStrength, motion.parallaxPixels,
                    motion.grainStrength, motion.printDriftPixels, motion.anomalyFrequency,
                    motion.swirlCenter.x, motion.swirlCenter.y)
                && Finite(
                    rain.density, rain.nearRatio, rain.length, rain.speed, rain.angleDeg, rain.wind,
                    rain.opacity, rain.glow, rain.menuProtect, rain.coreProtect, rain.gustChance, rain.gustStrength)
                && Finite(
                    scribble.minRegionWidth, scribble.leftOverflow, scribble.rightOverflow,
                    scribble.verticalPadding, scribble.regionHeight, scribble.drawDurationMs,
                    scribble.thickness, scribble.pressureVariation, scribble.tipResidualWidth,
                    scribble.entryLengthRatio, scribble.glowFadeAdvance, scribble.dryBrushFibers,
                    scribble.splatterSize, scribble.splatterSpread, scribble.wobble,
                    scribble.loopiness, scribble.turnAngleDeg, scribble.glow)
                && Finite(
                    interaction.hitPaddingX, interaction.hoverTextScale,
                    interaction.hoverEnterSeconds, interaction.hoverExitSeconds, interaction.pressScale,
                    interaction.confirmDurationSeconds, interaction.glowBreathAmount,
                    interaction.glowBreathPeriodSeconds, interaction.highlightPromptMinSeconds,
                    interaction.highlightPromptMaxSeconds, interaction.highlightPromptOffset,
                    interaction.highlightPromptDurationSeconds);
        }

        private static bool Finite(params float[] values)
        {
            for (int index = 0; index < values.Length; index++)
            {
                if (float.IsNaN(values[index]) || float.IsInfinity(values[index])) return false;
            }
            return true;
        }

        private static int RefreshOpenScenes(
            HallRainSettings rainSettings,
            MenuScribbleSettings scribbleSettings,
            MenuInteractionSettings interactionSettings,
            bool rainEnabled,
            bool reducedMotion,
            IEnumerable<ProceduralRainGraphic> recordedRainLayers,
            IEnumerable<ProceduralNeonScribbleGraphic> recordedScribbles)
        {
            int refreshedRainLayers = 0;
            foreach (ProceduralRainGraphic rainLayer in recordedRainLayers)
            {
                if (rainLayer == null || rainLayer.Settings != rainSettings) continue;
                rainLayer.SetRainEnabled(rainEnabled);
                rainLayer.SetReducedMotion(reducedMotion);
                rainLayer.Refresh();
                EditorUtility.SetDirty(rainLayer);
                MarkOwningSceneDirty(rainLayer.gameObject);
                refreshedRainLayers++;
            }

            foreach (ProceduralNeonScribbleGraphic scribble in recordedScribbles)
            {
                if (scribble.Settings != scribbleSettings && scribble.InteractionSettings != interactionSettings) continue;
                scribble.RefreshLayout(true);
                EditorUtility.SetDirty(scribble);
                MarkOwningSceneDirty(scribble.gameObject);
            }

            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
            return refreshedRainLayers;
        }

        private static T[] FindSceneObjects<T>() where T : UnityEngine.Object
        {
            return UnityEngine.Object.FindObjectsOfType<T>(true)
                .Where(item => item != null && !EditorUtility.IsPersistent(item))
                .ToArray();
        }

        private static void MarkAssetDirty(UnityEngine.Object target)
        {
            if (target != null && AssetDatabase.Contains(target))
            {
                EditorUtility.SetDirty(target);
            }
        }

        private static void MarkOwningSceneDirty(GameObject gameObject)
        {
            if (gameObject != null && gameObject.scene.IsValid() && gameObject.scene.isLoaded)
            {
                EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
        }

        private static ImportResult Fail(string message, string sourceVersion = null)
        {
            return new ImportResult(false, message, sourceVersion, false, 0);
        }
    }

    /// <summary>
    /// 面向策划/制作人的导入窗口。外部 JSON 可直接从 Downloads 选择，不要求先复制到 Assets。
    /// 四份目标资产会优先从当前打开场景的实际引用解析，其次才使用标准路径或唯一同类型资产。
    /// </summary>
    public sealed class HallMotionWebJsonImporterWindow : EditorWindow
    {
        private const string MotionSettingsPath = "Assets/HallMotion/Settings/HallMotionSettings.asset";
        private const string RainSettingsPath = "Assets/HallMotion/Settings/HallRainSettings.asset";
        private const string ScribbleSettingsPath = "Assets/HallMotion/Settings/MenuScribbleSettings.asset";
        private const string InteractionSettingsPath = "Assets/HallMotion/Settings/MenuInteractionSettings.asset";

        private string jsonPath;
        private Vector2 scroll;
        private HallMotionSettings motionSettings;
        private HallRainSettings rainSettings;
        private MenuScribbleSettings scribbleSettings;
        private MenuInteractionSettings interactionSettings;
        private MessageType resultType = MessageType.Info;
        private string resultMessage = "请选择网页实验台导出的完整 JSON。";

        [MenuItem("Tools/Miscalculation/Hall Motion/Import Web JSON...")]
        public static void OpenWindow()
        {
            HallMotionWebJsonImporterWindow window = GetWindow<HallMotionWebJsonImporterWindow>(true, "Import Hall Motion JSON", true);
            window.minSize = new Vector2(520f, 430f);
            window.Show();
        }

        private void OnEnable()
        {
            AutoResolveTargets();
        }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.LabelField("主界面动效 Web JSON → Unity", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "仅接受完整的 16:9 Web 导出文件。导入会同时更新四组 Settings；所有字段先通过临时副本校验，成功后才写入，且支持 Ctrl+Z 撤销。",
                MessageType.Info);

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("1. JSON 文件", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.SelectableLabel(string.IsNullOrEmpty(jsonPath) ? "尚未选择" : jsonPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("选择…", GUILayout.Width(84f)))
                {
                    SelectJson();
                }
            }

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("2. 写入目标", EditorStyles.boldLabel);
            motionSettings = (HallMotionSettings)EditorGUILayout.ObjectField("背景 Settings", motionSettings, typeof(HallMotionSettings), false);
            rainSettings = (HallRainSettings)EditorGUILayout.ObjectField("雨 Settings", rainSettings, typeof(HallRainSettings), false);
            scribbleSettings = (MenuScribbleSettings)EditorGUILayout.ObjectField("霓虹线 Settings", scribbleSettings, typeof(MenuScribbleSettings), false);
            interactionSettings = (MenuInteractionSettings)EditorGUILayout.ObjectField("菜单交互 Settings", interactionSettings, typeof(MenuInteractionSettings), false);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("从打开场景自动识别"))
                {
                    AutoResolveTargets();
                }
                if (GUILayout.Button("选中四份资产"))
                {
                    Selection.objects = new UnityEngine.Object[] { motionSettings, rainSettings, scribbleSettings, interactionSettings }
                        .Where(item => item != null)
                        .ToArray();
                }
            }

            EditorGUILayout.Space(12f);
            EditorGUILayout.HelpBox(resultMessage, resultType);

            bool ready = !string.IsNullOrEmpty(jsonPath)
                && motionSettings != null
                && rainSettings != null
                && scribbleSettings != null
                && interactionSettings != null;
            using (new EditorGUI.DisabledScope(!ready || EditorApplication.isPlayingOrWillChangePlaymode))
            {
                if (GUILayout.Button("校验并导入四组参数", GUILayout.Height(34f)))
                {
                    ImportSelectedJson();
                }
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.HelpBox("请先停止 Play Mode。", MessageType.Warning);
            }
            EditorGUILayout.EndScrollView();
        }

        private void SelectJson()
        {
            string initialDirectory = string.IsNullOrEmpty(jsonPath)
                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                : Path.GetDirectoryName(jsonPath);
            string selected = EditorUtility.OpenFilePanel("选择 Hall Motion Web JSON", initialDirectory, "json");
            if (string.IsNullOrEmpty(selected)) return;

            jsonPath = selected;
            resultType = MessageType.Info;
            resultMessage = "文件已选择；点击下方按钮后才会修改 Settings。";
        }

        private void ImportSelectedJson()
        {
            string json;
            try
            {
                json = File.ReadAllText(jsonPath);
            }
            catch (Exception exception)
            {
                resultType = MessageType.Error;
                resultMessage = "无法读取 JSON：" + exception.Message;
                return;
            }

            HallMotionWebJsonImporter.ImportResult result = HallMotionWebJsonImporter.ImportJson(
                json,
                motionSettings,
                rainSettings,
                scribbleSettings,
                interactionSettings);
            resultType = result.Success ? (result.CompatibilityMode ? MessageType.Warning : MessageType.Info) : MessageType.Error;
            resultMessage = result.Message;
            if (result.Success)
            {
                Debug.Log($"[Hall Motion JSON Import] PASS — source v{result.SourceVersion}. {result.Message}");
            }
            else
            {
                Debug.LogError("[Hall Motion JSON Import] " + result.Message);
            }
        }

        private void AutoResolveTargets()
        {
            motionSettings = FindReferencedMotionSettings() ?? FindAsset<HallMotionSettings>(MotionSettingsPath);
            rainSettings = FindReferencedRainSettings() ?? FindAsset<HallRainSettings>(RainSettingsPath);
            scribbleSettings = FindReferencedScribbleSettings() ?? FindAsset<MenuScribbleSettings>(ScribbleSettingsPath);
            interactionSettings = FindReferencedInteractionSettings() ?? FindAsset<MenuInteractionSettings>(InteractionSettingsPath);
            Repaint();
        }

        private static HallMotionSettings FindReferencedMotionSettings()
        {
            return UniqueOrNull(UnityEngine.Object.FindObjectsOfType<HallMotionController>(true)
                .Where(controller => !EditorUtility.IsPersistent(controller) && controller.Settings != null)
                .Select(controller => controller.Settings)
                .Distinct());
        }

        private static HallRainSettings FindReferencedRainSettings()
        {
            return UniqueOrNull(UnityEngine.Object.FindObjectsOfType<ProceduralRainGraphic>(true)
                .Where(graphic => !EditorUtility.IsPersistent(graphic) && graphic.Settings != null)
                .Select(graphic => graphic.Settings)
                .Distinct());
        }

        private static MenuScribbleSettings FindReferencedScribbleSettings()
        {
            return UniqueOrNull(UnityEngine.Object.FindObjectsOfType<ProceduralNeonScribbleGraphic>(true)
                .Where(graphic => !EditorUtility.IsPersistent(graphic) && graphic.Settings != null)
                .Select(graphic => graphic.Settings)
                .Distinct());
        }

        private static MenuInteractionSettings FindReferencedInteractionSettings()
        {
            return UniqueOrNull(UnityEngine.Object.FindObjectsOfType<ProceduralNeonScribbleGraphic>(true)
                .Where(graphic => !EditorUtility.IsPersistent(graphic) && graphic.InteractionSettings != null)
                .Select(graphic => graphic.InteractionSettings)
                .Distinct());
        }

        private static T UniqueOrNull<T>(IEnumerable<T> values) where T : UnityEngine.Object
        {
            T[] candidates = values.Take(2).ToArray();
            return candidates.Length == 1 ? candidates[0] : null;
        }

        private static T FindAsset<T>(string preferredPath) where T : UnityEngine.Object
        {
            T preferred = AssetDatabase.LoadAssetAtPath<T>(preferredPath);
            if (preferred != null) return preferred;

            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            if (guids.Length != 1) return null;
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
