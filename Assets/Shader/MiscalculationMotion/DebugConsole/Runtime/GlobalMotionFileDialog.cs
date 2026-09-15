using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Miscalculation.Motion.DebugConsole
{
    /// <summary>Editor 由可选 Editor 程序集注入；Windows Player 使用系统 JSON 文件对话框。</summary>
    public static class GlobalMotionFileDialog
    {
        public static Func<bool, string, string, string> Override;

        public static string Choose(bool save, string directory, string filename)
        {
            if (Override != null) return Override(save, directory, filename);
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            var data = new OpenFileName
            {
                size = Marshal.SizeOf(typeof(OpenFileName)), owner = GetActiveWindow(), filter = "JSON 参数\0*.json\0\0",
                file = new StringBuilder(filename, 4096), maxFile = 4096, initialDir = directory,
                title = save ? "导出全局动效参数" : "导入全局动效参数", defaultExt = "json",
                flags = 0x00080000 | 0x00000008 | (save ? 0x00000002 : 0x00001000)
            };
            bool result = save ? GetSaveFileName(data) : GetOpenFileName(data);
            if (result) return data.file.ToString();
            int error = CommDlgExtendedError();
            if (error != 0) throw new InvalidOperationException("文件对话框错误：" + error);
            return null;
#else
            throw new PlatformNotSupportedException("请为当前平台注入 GlobalMotionFileDialog.Override。");
#endif
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        sealed class OpenFileName
        {
            public int size; public IntPtr owner, instance; public string filter; public IntPtr customFilter;
            public int maxCustomFilter, filterIndex; public StringBuilder file; public int maxFile; public IntPtr fileTitle;
            public int maxFileTitle; public string initialDir, title; public int flags; public short fileOffset, fileExtension;
            public string defaultExt; public IntPtr custData, hook, templateName, pvReserved; public int dwReserved, flagsEx;
        }

        [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] static extern bool GetOpenFileName([In, Out] OpenFileName data);
        [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)] [return: MarshalAs(UnmanagedType.Bool)] static extern bool GetSaveFileName([In, Out] OpenFileName data);
        [DllImport("comdlg32.dll")] static extern int CommDlgExtendedError();
        [DllImport("user32.dll")] static extern IntPtr GetActiveWindow();
    }
}
