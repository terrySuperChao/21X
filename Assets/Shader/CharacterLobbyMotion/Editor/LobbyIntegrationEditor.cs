using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Miscalculation.CharacterLobby.EditorTools
{
    /// <summary>随功能源码交付；无需验证场景即可使用现有 Settings、Controller 和 JSON。</summary>
    public static class LobbyIntegrationEditor
    {
        [MenuItem("Tools/Miscalculation/Character Lobby/Import JSON into selected Settings")]
        public static void ImportSelected()
        {
            var settings=Selection.activeObject as LobbySettings;
            if(!settings){EditorUtility.DisplayDialog("大厅 JSON","请先选择项目自有的 LobbySettings 资产。","确定");return;}
            string[] ids=AssetDatabase.FindAssets("t:LobbyArt");
            if(ids.Length!=1){EditorUtility.DisplayDialog("大厅 JSON","请使用 Controller Inspector 的导入按钮（显式绑定 LobbyArt），或保证项目只有一个 LobbyArt。","确定");return;}
            Import(settings,AssetDatabase.LoadAssetAtPath<LobbyArt>(AssetDatabase.GUIDToAssetPath(ids[0])));
        }
        public static void Import(LobbySettings settings,LobbyArt art)
        {
            if(!settings||!art){EditorUtility.DisplayDialog("大厅 JSON","先绑定 Settings 和 Art。","确定");return;}
            string path=EditorUtility.OpenFilePanel("导入完整大厅 JSON","","json");if(string.IsNullOrEmpty(path))return;
            try{LobbyPreset p=LobbyPreset.ReadFile(path,art);Undo.RecordObject(settings,"Import Lobby JSON");settings.parameters=p.parameters.Clone();EditorUtility.SetDirty(settings);AssetDatabase.SaveAssetIfDirty(settings);Debug.Log("[Character Lobby] "+p.importNote);}
            catch(Exception e){EditorUtility.DisplayDialog("导入失败（未修改）",e.Message,"确定");}
        }
        public static string Validate(LobbyController c)
        {
            if(!c.art||!c.art.IsValid)return "缺少 Art、材质、背景或 24 帧烟雾元数据。";
            if(!c.designRoot||!c.designRoot.GetComponentInParent<Canvas>())return "需要在现有 Canvas 下指定 1920×1080 设计根。";
            if(c.art.anchors.Length!=24)return "烟雾锚点数量错误。";
            foreach(var a in c.art.anchors)if(!LobbyMath.Finite(a.x)||!LobbyMath.Finite(a.y)||a.x<0||a.x>=512||a.y<0||a.y>=512)return "非法烟雾锚点。";
            if(QualitySettings.activeColorSpace!=ColorSpace.Gamma)return "当前不是 Gamma。不要自动修改目标工程；如需同观感请先协商渲染条件。";
            try{(c.defaults?c.defaults.parameters:new LobbyParameters()).Validate();}catch(Exception e){return e.Message;}
            return "OK：不替换 Canvas/EventSystem/业务按钮。请确认根节点与背景使用同一适配策略。";
        }
    }
    [CustomEditor(typeof(LobbyController))]
    public sealed class LobbyControllerInspector:UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();var c=(LobbyController)target;
            EditorGUILayout.Space();EditorGUILayout.HelpBox("大厅动效 v"+LobbySettings.Version+"。先绑定现有 Canvas 下的设计根；控制台、背景与验证工程均可独立选用。",MessageType.Info);
            if(GUILayout.Button("检查现有场景接入"))EditorUtility.DisplayDialog("接入检查",LobbyIntegrationEditor.Validate(c),"确定");
            if(GUILayout.Button("导入 JSON 到绑定的 Settings"))LobbyIntegrationEditor.Import(c.defaults,c.art);
            if(Application.isPlaying&&c.Current!=null){
                if(GUILayout.Button("保存当前运行参数为新的 Settings 资产")){
                    string path=EditorUtility.SaveFilePanelInProject("保存当前参数","Lobby_Custom_Settings","asset","参数保存为项目自有资产，不覆盖出厂设置。");
                    if(!string.IsNullOrEmpty(path)){var asset=CreateInstance<LobbySettings>();asset.parameters=c.Current.Clone();AssetDatabase.CreateAsset(asset,path);AssetDatabase.SaveAssets();}
                }
                if(GUILayout.Button("台灯切换（公开 API）"))c.SetLamp(!c.Current.lampOn);
            }
        }
    }
}
