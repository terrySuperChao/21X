using Pb;
using System.IO;
using UnityEngine;
public class ProtobufMgr : Singleton<ProtobufMgr>
{
    // 序列化到二进制文件
    public void serializeToFile(string path,byte[] data)
    {
        File.WriteAllBytes(path, data);
    }

    // 从二进制文件反序列化
    public FileStream deserializeFromFile(string path)
    {
        if (File.Exists(path)) {
            return File.OpenRead(path);
        }
        return null;
    }
}