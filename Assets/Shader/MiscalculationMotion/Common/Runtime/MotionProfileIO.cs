using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Miscalculation.Motion.Common
{
    public static class MotionProfileIO
    {
        public const int MaxJsonBytes = 128 * 1024;

        public static CommonMotionProfile ParseCommon(string json, out string note)
        {
            if (string.IsNullOrWhiteSpace(json) || Encoding.UTF8.GetByteCount(json) > MaxJsonBytes)
                throw new ArgumentException("JSON 为空或超过 128KB");
            var current = new CommonMotionProfile();
            JsonUtility.FromJsonOverwrite(json, current);
            bool migratedLegacyMemory = json.IndexOf("\"memory\"", StringComparison.Ordinal) >= 0 && json.IndexOf("\"itemMemory\"", StringComparison.Ordinal) < 0;
            bool migratedLegacyStamp = json.IndexOf("\"startHeightPx\"", StringComparison.Ordinal) >= 0 && json.IndexOf("\"perspectiveOffsetY\"", StringComparison.Ordinal) < 0;
            bool migratedPerspectiveStamp = json.IndexOf("\"perspectiveOffsetY\"", StringComparison.Ordinal) >= 0 && json.IndexOf("\"heightProjectionY\"", StringComparison.Ordinal) < 0;
            bool migratedSharedSelection = json.IndexOf("\"selection\"", StringComparison.Ordinal) >= 0 && json.IndexOf("\"difficultySelection\"", StringComparison.Ordinal) < 0;
            if (migratedLegacyMemory)
            {
                LegacyCommonMotionProfile legacy = JsonUtility.FromJson<LegacyCommonMotionProfile>(json);
                if (legacy != null && legacy.memory != null)
                {
                    current.itemMemory = CloneMemory(legacy.memory);
                    current.cardArtworkMemory = CloneMemory(legacy.memory);
                }
            }
            if (migratedLegacyStamp)
            {
                LegacyCommonMotionProfile legacy = JsonUtility.FromJson<LegacyCommonMotionProfile>(json);
                if (legacy != null && legacy.transfer != null)
                {
                    float distance = Mathf.Clamp(legacy.transfer.startHeightPx, 20f, 900f);
                    current.transfer.heightProjectionY = Mathf.Clamp(distance * .08f, 0f, 80f);
                    current.transfer.startScale = Mathf.Clamp(legacy.transfer.startScale + .85f, 1.45f, 4f);
                }
            }
            else if (migratedPerspectiveStamp)
            {
                LegacyCommonMotionProfile legacy = JsonUtility.FromJson<LegacyCommonMotionProfile>(json);
                if (legacy != null && legacy.transfer != null)
                    current.transfer.heightProjectionY = Mathf.Clamp(-legacy.transfer.perspectiveOffsetY * .12f, -80f, 80f);
            }
            if (migratedSharedSelection)
            {
                LegacyCommonMotionProfile legacy = JsonUtility.FromJson<LegacyCommonMotionProfile>(json);
                if (legacy != null && legacy.selection != null)
                {
                    current.difficultySelection = CloneSelection(legacy.selection, legacy.selection.difficultyColor, "#440006");
                    current.chapterSelection = CloneSelection(legacy.selection, legacy.selection.chapterNodeColor, "#7B0611");
                }
            }
            current.Validate();
            bool migrated = migratedLegacyMemory || migratedLegacyStamp || migratedPerspectiveStamp || migratedSharedSelection || current.motionVersion != CommonMotionProfile.CurrentVersion;
            note = migrated ? "已按相同 Schema 兼容导入，新增字段使用当前默认值。" : "版本与 Schema 匹配。";
            current.motionVersion = CommonMotionProfile.CurrentVersion;
            current.algorithmVersion = "common-motion-v1.0.4-card-contact-and-safe-release";
            return current;
        }

        [Serializable]
        sealed class LegacyCommonMotionProfile { public MemoryTransitionSettings memory; public LegacyImageTransferSettings transfer; public LegacySelectionSettings selection; }
        [Serializable]
        sealed class LegacyImageTransferSettings { public float startHeightPx = 210f; public float perspectiveOffsetY = -170f; public float startScale = 1.12f; }
        [Serializable]
        sealed class LegacySelectionSettings
        {
            public float durationMs = 720f, strokeWidth = 3.8f, edgeFeather = 1.15f, taperLength = 18f, paddingX = 18f, paddingY = 10f;
            public string difficultyColor = "#440006", chapterNodeColor = "#7B0611";
        }
        static MemoryTransitionSettings CloneMemory(MemoryTransitionSettings value) => JsonUtility.FromJson<MemoryTransitionSettings>(JsonUtility.ToJson(value));
        static SelectionMotionSettings CloneSelection(LegacySelectionSettings value, string color, string fallback)
        {
            return new SelectionMotionSettings
            {
                durationMs = value.durationMs, strokeWidth = value.strokeWidth, edgeFeather = value.edgeFeather,
                taperLength = value.taperLength, paddingX = value.paddingX, paddingY = value.paddingY,
                color = string.IsNullOrEmpty(color) ? fallback : color
            };
        }

        public static string ToJson(CommonMotionProfile profile)
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));
            profile.Validate();
            return JsonUtility.ToJson(profile, true);
        }

        public static string ReadAll(string path)
        {
            var info = new FileInfo(path);
            if (!info.Exists) throw new FileNotFoundException("找不到参数文件", path);
            if (info.Length > MaxJsonBytes) throw new ArgumentException("参数文件超过 128KB");
            return File.ReadAllText(info.FullName, Encoding.UTF8);
        }

        public static void WriteAtomic(string path, string json)
        {
            string full = Path.GetFullPath(path);
            string directory = Path.GetDirectoryName(full);
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory)) throw new DirectoryNotFoundException(directory);
            string temporary = full + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.WriteAllText(temporary, json, new UTF8Encoding(false));
                if (File.Exists(full)) File.Replace(temporary, full, null);
                else File.Move(temporary, full);
            }
            finally
            {
                if (File.Exists(temporary)) File.Delete(temporary);
            }
        }
    }

    [CreateAssetMenu(menuName = "Miscalculation Motion/Common Profile")]
    public sealed class CommonMotionProfileAsset : ScriptableObject
    {
        public CommonMotionProfile parameters = new CommonMotionProfile();
    }
}
