using UnityEditor;

namespace Miscalculation.Motion.DebugConsole
{
    [InitializeOnLoad]
    public static class GlobalMotionEditorFileDialog
    {
        static GlobalMotionEditorFileDialog()
        {
            GlobalMotionFileDialog.Override = (save, directory, filename) => save
                ? EditorUtility.SaveFilePanel("导出全局动效参数", directory, filename, "json")
                : EditorUtility.OpenFilePanel("导入全局动效参数", directory, "json");
        }
    }
}
