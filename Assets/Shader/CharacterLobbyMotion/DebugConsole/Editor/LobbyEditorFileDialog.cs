using UnityEditor;
namespace Miscalculation.CharacterLobby.Debugging
{
    [InitializeOnLoad]
    public static class LobbyEditorFileDialog
    {
        static LobbyEditorFileDialog(){LobbyFileDialog.Override=(save,dir,file)=>save?EditorUtility.SaveFilePanel("导出大厅参数",dir,file,"json"):EditorUtility.OpenFilePanel("导入大厅参数",dir,"json");}
    }
}
