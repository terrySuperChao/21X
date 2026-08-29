using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class HallMotionURPSetup
{
    private const string SettingsDirectory = "Assets/Settings";
    private const string RendererPath = SettingsDirectory + "/HallMotionURPRenderer.asset";
    private const string PipelinePath = SettingsDirectory + "/HallMotionURPAsset.asset";

    [MenuItem("Tools/Hall Motion/Configure URP")]
    public static void Configure()
    {
        Directory.CreateDirectory(SettingsDirectory);

        var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
        if (renderer == null)
        {
            renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
            AssetDatabase.CreateAsset(renderer, RendererPath);
        }

        var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
        if (pipeline == null)
        {
            pipeline = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
            AssetDatabase.CreateAsset(pipeline, PipelinePath);
        }

        var serializedPipeline = new SerializedObject(pipeline);
        var rendererList = serializedPipeline.FindProperty("m_RendererDataList");
        rendererList.arraySize = 1;
        rendererList.GetArrayElementAtIndex(0).objectReferenceValue = renderer;
        serializedPipeline.FindProperty("m_DefaultRendererIndex").intValue = 0;
        serializedPipeline.ApplyModifiedPropertiesWithoutUndo();

        GraphicsSettings.renderPipelineAsset = pipeline;
        for (var i = 0; i < QualitySettings.names.Length; i++)
        {
            QualitySettings.SetQualityLevel(i, false);
            QualitySettings.renderPipeline = pipeline;
        }

        EditorUtility.SetDirty(pipeline);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Hall Motion URP configured successfully.");
    }
}
