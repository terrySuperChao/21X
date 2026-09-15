using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
namespace Miscalculation.CharacterLobby
{
    [Serializable]
    public sealed class LobbyPreset
    {
        public const int CurrentSchema=1;
        public int schemaVersion=CurrentSchema;
        public string effectVersion=LobbySettings.Version, algorithmVersion=LobbySettings.Algorithm, assetVersion, assetHash;
        public int referenceWidth=1920, referenceHeight=1080;
        public string presetName="大厅自定义参数";
        public LobbyParameters parameters;
        [NonSerialized] public bool wasMigrated;
        [NonSerialized] public string importNote;
        public static LobbyPreset Create(LobbyParameters p, LobbyArt art, string name)
        { return new LobbyPreset {parameters=p.Clone(),assetVersion=art.assetVersion,assetHash=art.contentHash,presetName=name}; }
        public string ToJson() { parameters.Validate(); return JsonUtility.ToJson(this,true); }
        public static LobbyPreset Parse(string json,LobbyArt art)
        {
            if (string.IsNullOrEmpty(json)||json.Length>65536) throw new ArgumentException("JSON 为空或超过 64KB");
            // 已知字段仍严格检查类型与重复项；新增/删除字段采用兼容迁移。
            // 真正不兼容的参数语义必须提升 schemaVersion，而非只提升效果版本号。
            var structure=new StrictJson(json);structure.Read(typeof(LobbyPreset));
            if(!structure.RootFields.Contains("schemaVersion")||!structure.RootFields.Contains("parameters"))throw new ArgumentException("缺少 Schema 或参数主体，未修改当前参数");
            var p=new LobbyPreset {parameters=new LobbyParameters()};JsonUtility.FromJsonOverwrite(json,p);
            if(p.schemaVersion!=CurrentSchema)throw new ArgumentException("参数 Schema 不兼容（文件 "+p.schemaVersion+"，当前 "+CurrentSchema+"），未修改当前参数");
            if(p.referenceWidth!=1920||p.referenceHeight!=1080)throw new ArgumentException("美术坐标语义不兼容，未修改当前参数");
            if(string.IsNullOrWhiteSpace(p.presetName)||p.presetName.Length>120)throw new ArgumentException("预设名称无效");
            p.parameters.Validate();
            string oldEffect=p.effectVersion,oldAlgorithm=p.algorithmVersion,oldAsset=p.assetVersion,oldHash=p.assetHash;var reasons=new List<string>();
            if(oldEffect!=LobbySettings.Version)reasons.Add("效果版本 "+(string.IsNullOrEmpty(oldEffect)?"未知":oldEffect)+" → "+LobbySettings.Version);
            if(oldAlgorithm!=LobbySettings.Algorithm)reasons.Add("算法标识已更新");
            if(oldAsset!=art.assetVersion||oldHash!=art.contentHash)reasons.Add("素材标识已更新，参数仍兼容");
            int missing=0;foreach(FieldInfo f in typeof(LobbyParameters).GetFields(BindingFlags.Public|BindingFlags.Instance))if(!f.IsNotSerialized&&!structure.ParameterFields.Contains(f.Name))missing++;
            if(missing>0)reasons.Add(missing+" 个新增参数使用当前默认值");if(structure.UnknownFieldCount>0)reasons.Add(structure.UnknownFieldCount+" 个未知字段已忽略");
            p.wasMigrated=reasons.Count>0;p.importNote=p.wasMigrated?"已兼容迁移："+string.Join("；",reasons.ToArray()):"版本与参数完全匹配";
            p.effectVersion=LobbySettings.Version;p.algorithmVersion=LobbySettings.Algorithm;p.assetVersion=art.assetVersion;p.assetHash=art.contentHash;return p;
        }
        public static LobbyPreset ReadFile(string path,LobbyArt art)
        { if(new FileInfo(path).Length>65536)throw new ArgumentException("预设文件超过 64KB");return Parse(File.ReadAllText(path,Encoding.UTF8),art); }
        public static void WriteFile(string path,string json)
        {
            string full=Path.GetFullPath(path),dir=Path.GetDirectoryName(full);
            if(!Directory.Exists(dir))throw new DirectoryNotFoundException(dir);
            // 同目录临时文件 + 原子替换；失败不清空已有预设。
            string tmp=full+"."+Guid.NewGuid().ToString("N")+".tmp";
            try {File.WriteAllText(tmp,json,new UTF8Encoding(false));if(File.Exists(full))File.Replace(tmp,full,null);else File.Move(tmp,full);}
            finally {if(File.Exists(tmp))File.Delete(tmp);}
        }
    }
    internal sealed class StrictJson
    {
        readonly string s;int i;
        public readonly HashSet<string> RootFields=new HashSet<string>(),ParameterFields=new HashSet<string>();
        public int UnknownFieldCount {get;private set;}
        public StrictJson(string value){s=value;}
        void Space(){while(i<s.Length&&char.IsWhiteSpace(s[i]))i++;}
        bool Take(char c){Space();if(i<s.Length&&s[i]==c){i++;return true;}return false;}
        void Need(char c){if(!Take(c))throw new ArgumentException("JSON 格式错误，位置 "+i);}
        string Str()
        {
            Need('"');var b=new StringBuilder();bool end=false;
            while(i<s.Length){char c=s[i++];if(c=='"'){end=true;break;}if(c<32)throw new ArgumentException("JSON 非法字符");
                if(c=='\\'){if(i>=s.Length)throw new ArgumentException("JSON 转义不完整");char e=s[i++];switch(e){case '"':c='"';break;case '\\':c='\\';break;case '/':c='/';break;case 'b':c='\b';break;case 'f':c='\f';break;case 'n':c='\n';break;case 'r':c='\r';break;case 't':c='\t';break;case 'u':if(i+4>s.Length)throw new ArgumentException("JSON unicode 不完整");c=(char)int.Parse(s.Substring(i,4),NumberStyles.HexNumber);i+=4;break;default:throw new ArgumentException("JSON 非法转义");}}
                b.Append(c);}
            if(!end)throw new ArgumentException("JSON 字符串未关闭");return b.ToString();
        }
        void Number(Type type)
        {
            int start=i;if(i<s.Length&&s[i]=='-')i++;if(i>=s.Length||s[i]<'0'||s[i]>'9')throw new ArgumentException("预期数字");
            if(s[i]=='0')i++;else while(i<s.Length&&char.IsDigit(s[i]))i++;
            if(i<s.Length&&s[i]=='.'){i++;int a=i;while(i<s.Length&&char.IsDigit(s[i]))i++;if(a==i)throw new ArgumentException("小数不完整");}
            if(i<s.Length&&(s[i]=='e'||s[i]=='E')){i++;if(i<s.Length&&(s[i]=='+'||s[i]=='-'))i++;int a=i;while(i<s.Length&&char.IsDigit(s[i]))i++;if(a==i)throw new ArgumentException("指数不完整");}
            string num=s.Substring(start,i-start);double d;if(!double.TryParse(num,NumberStyles.Float,CultureInfo.InvariantCulture,out d)||double.IsNaN(d)||double.IsInfinity(d))throw new ArgumentException("JSON 数值无效或溢出");
            if(type==typeof(float)&&Math.Abs(d)>float.MaxValue)throw new ArgumentException("JSON 浮点数溢出");
            if(type==typeof(uint)&&(d<0||d>uint.MaxValue||Math.Floor(d)!=d))throw new ArgumentException("非法随机种子");if(type==typeof(int)&&(d<int.MinValue||d>int.MaxValue||Math.Floor(d)!=d))throw new ArgumentException("非法整数");
        }
        bool Literal(string value){Space();if(i+value.Length>s.Length||string.CompareOrdinal(s,i,value,0,value.Length)!=0)return false;i+=value.Length;return true;}
        void SkipValue()
        {
            Space();if(i>=s.Length)throw new ArgumentException("JSON 值不完整");if(s[i]=='\"'){Str();return;}
            if(Take('{')){var seen=new HashSet<string>();if(!Take('}')){do{string key=Str();if(!seen.Add(key))throw new ArgumentException("未知对象含重复字段："+key);Need(':');SkipValue();}while(Take(','));Need('}');}return;}
            if(Take('[')){if(!Take(']')){do{SkipValue();}while(Take(','));Need(']');}return;}
            if(Literal("true")||Literal("false")||Literal("null"))return;Number(typeof(double));
        }
        void Value(Type type)
        {
            Space();if(type==typeof(string)){Str();return;}if(type==typeof(bool)){if(!Literal("true")&&!Literal("false"))throw new ArgumentException("预期布尔值");return;}
            if(type==typeof(float)||type==typeof(uint)||type==typeof(int)){Number(type);return;}
            Need('{');var seen=new HashSet<string>();
            if(!Take('}')){do{string key=Str();if(!seen.Add(key))throw new ArgumentException("重复字段："+key);Need(':');FieldInfo f=type.GetField(key,BindingFlags.Public|BindingFlags.Instance);
                if(f==null||f.IsNotSerialized){UnknownFieldCount++;SkipValue();}else Value(f.FieldType);}while(Take(','));Need('}');}
            if(type==typeof(LobbyPreset))RootFields.UnionWith(seen);else if(type==typeof(LobbyParameters))ParameterFields.UnionWith(seen);
        }
        public void Read(Type t){Value(t);Space();if(i!=s.Length)throw new ArgumentException("JSON 尾部有多余内容");}
    }
}
