using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TriggerPartInfo
{
    public int id;
    public string name;
    public int type;
    public int value;
}

public class TriggerPartConfig
{
    private readonly string _path = "config/triggerPart";
    private List<TriggerPartInfo> _list = null;
    public void init()
    {
        this._list = JsonMgr.Instance.readObject<List<TriggerPartInfo>>(this._path);
    }

    public List<TriggerPartInfo> getTriggerPart() {
        return this._list;
    }

    public TriggerPartInfo getTriggerPartId(int id) { 
        return this._list.Find(x => x.id == id);
    }
}
