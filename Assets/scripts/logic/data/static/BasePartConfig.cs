using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class BasePartInfo
{
    public int id;
    public string name;
    public int level;
    public int type;
    public int value;
}

public class BasePartConfig
{
    private readonly string _path = "config/basePart";
    private List<BasePartInfo> _list = null;
    public void init()
    {
        this._list = JsonMgr.Instance.readObject<List<BasePartInfo>>(this._path);
    }

    public List<BasePartInfo> getBasePart() {
        return this._list;
    }

    public BasePartInfo getBasePartId(int id) { 
        return this._list.Find(x => x.id == id);
    }
}
