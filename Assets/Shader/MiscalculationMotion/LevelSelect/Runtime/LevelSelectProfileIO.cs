using System;
using UnityEngine;

namespace Miscalculation.Motion.LevelSelect
{
    public static class LevelSelectProfileIO
    {
        public const string CurrentVersion = "1.0.4";
        public static LevelSelectMotionProfile ImportCompatible(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || json.Length > 65536) throw new ArgumentException("关卡交互预设为空或过大");
            LevelSelectMotionProfile value = new LevelSelectMotionProfile(); JsonUtility.FromJsonOverwrite(json, value);
            if (value.schemaVersion != 1) throw new ArgumentException("关卡交互预设 Schema 不兼容"); value.Validate(); value.motionVersion = CurrentVersion; return value;
        }
        public static string Export(LevelSelectMotionProfile value) { value = value ?? new LevelSelectMotionProfile(); value.Validate(); return JsonUtility.ToJson(value, true); }
    }
}
