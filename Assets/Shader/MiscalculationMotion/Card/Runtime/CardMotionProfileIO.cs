using System;
using System.IO;
using UnityEngine;

namespace Miscalculation.Motion.Card
{
    public static class CardMotionProfileIO
    {
        public static CardMotionProfile ImportCompatible(string json)
        {
            if (string.IsNullOrWhiteSpace(json) || json.Length > 131072) throw new ArgumentException("发牌预设为空或过大");
            CardMotionProfile value = new CardMotionProfile();
            JsonUtility.FromJsonOverwrite(json, value);
            if (value.schemaVersion != 1) throw new ArgumentException("发牌预设 Schema 不兼容");
            value.ClampInPlace(); return value;
        }
        public static string Export(CardMotionProfile value, bool pretty = true) { value = value ?? new CardMotionProfile(); value.ClampInPlace(); return JsonUtility.ToJson(value, pretty); }
        public static void SaveAtomic(string path, CardMotionProfile value)
        {
            string full = Path.GetFullPath(path); Directory.CreateDirectory(Path.GetDirectoryName(full)); string temp = full + ".tmp";
            File.WriteAllText(temp, Export(value)); if (File.Exists(full)) File.Replace(temp, full, null); else File.Move(temp, full);
        }
    }
}
