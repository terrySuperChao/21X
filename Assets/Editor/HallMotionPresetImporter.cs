using System;
using System.IO;
using Miscalculation.HallMotion;
using UnityEditor;
using UnityEngine;

public static class HallMotionPresetImporter
{
    private const string MenuPath = "Tools/Hall Motion/Import Preset JSON...";
    private const string PresetDirectory = "Assets/Shader/HallMotion/Presets";
    private const string ImportedPresetPath = PresetDirectory + "/hall-motion-custom.json";
    private const string HallSettingsPath = "Assets/Shader/Material/HallMotionSettings_Standard.asset";
    private const string ScribbleSettingsPath = "Assets/Shader/Material/MenuScribbleSettings.asset";
    private const string InteractionSettingsPath = "Assets/Shader/Material/MenuInteractionSettings.asset";
    private const string RainSettingsPath = "Assets/Shader/Material/HallRainSettings.asset";
    private const string SupportedSchema = "com.miscalculation.hall-motion/v3";

    [Serializable]
    private sealed class PresetHeader
    {
        public string schema;
        public string motionVersion;
    }

    [MenuItem(MenuPath)]
    private static void ImportPreset()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("Hall Motion", "请先退出 Play Mode，再导入参数。", "确定");
            return;
        }

        string sourcePath = EditorUtility.OpenFilePanel("选择 Hall Motion JSON", string.Empty, "json");
        if (string.IsNullOrEmpty(sourcePath)) return;

        try
        {
            string json = File.ReadAllText(sourcePath);
            PresetHeader header = JsonUtility.FromJson<PresetHeader>(json);
            if (header == null || !string.Equals(header.schema, SupportedSchema, StringComparison.Ordinal))
            {
                EditorUtility.DisplayDialog(
                    "Hall Motion 导入失败",
                    $"不支持的 Schema：{header?.schema ?? "<empty>"}\n需要：{SupportedSchema}",
                    "确定");
                return;
            }

            HallMotionSettings hall = LoadRequired<HallMotionSettings>(HallSettingsPath);
            MenuScribbleSettings scribble = LoadRequired<MenuScribbleSettings>(ScribbleSettingsPath);
            MenuInteractionSettings interaction = LoadRequired<MenuInteractionSettings>(InteractionSettingsPath);
            HallRainSettings rain = LoadRequired<HallRainSettings>(RainSettingsPath);

            UnityEngine.Object[] targets = { hall, scribble, interaction, rain };
            Undo.RecordObjects(targets, "Import Hall Motion Preset");

            hall.ApplyJson(json);
            scribble.ApplyWebJson(json);
            interaction.ApplyWebJson(json);
            rain.ApplyWebJson(json);

            foreach (UnityEngine.Object target in targets) EditorUtility.SetDirty(target);
            SavePresetCopy(sourcePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string versionNote = string.Equals(header.motionVersion, HallMotionSettings.MotionVersion, StringComparison.Ordinal)
                ? string.Empty
                : $"\n\n注意：JSON 版本为 {header.motionVersion}，当前运行时代码为 {HallMotionSettings.MotionVersion}；参数已按 v3 Schema 兼容导入。";
            EditorUtility.DisplayDialog(
                "Hall Motion 导入完成",
                "已更新背景、菜单划线、菜单交互和雨效四份 Settings。\n" +
                $"预设副本：{ImportedPresetPath}" + versionNote,
                "确定");
            Debug.Log($"Hall Motion preset imported from '{sourcePath}'.{versionNote}");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorUtility.DisplayDialog("Hall Motion 导入失败", exception.Message, "确定");
        }
    }

    [MenuItem(MenuPath, true)]
    private static bool ValidateImportPreset()
    {
        return !EditorApplication.isPlayingOrWillChangePlaymode;
    }

    private static T LoadRequired<T>(string assetPath) where T : UnityEngine.Object
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (asset == null)
        {
            throw new FileNotFoundException($"找不到配置资产：{assetPath}");
        }
        return asset;
    }

    private static void SavePresetCopy(string sourcePath)
    {
        string targetAbsolutePath = Path.GetFullPath(ImportedPresetPath);
        string sourceAbsolutePath = Path.GetFullPath(sourcePath);
        Directory.CreateDirectory(Path.GetDirectoryName(targetAbsolutePath));
        if (!string.Equals(sourceAbsolutePath, targetAbsolutePath, StringComparison.Ordinal))
        {
            File.Copy(sourceAbsolutePath, targetAbsolutePath, true);
        }
        AssetDatabase.ImportAsset(ImportedPresetPath, ImportAssetOptions.ForceUpdate);
    }
}
