using UnityEngine;
using Newtonsoft.Json;

public class JsonMgr : Singleton<JsonMgr>
{
    public T readObject<T>(string path) {
        TextAsset jsonFile = Resources.Load<TextAsset>(path);
        T t = JsonConvert.DeserializeObject<T>(jsonFile.text);
        if (t == null) {
            Debug.Log(path + "read error");
            t = (T)new object();
        }
        return t;
    }
}
