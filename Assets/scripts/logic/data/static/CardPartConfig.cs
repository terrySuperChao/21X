using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CardPartInfo
{
    public int basePartId;
    public List<int> upgradePartIds;
    public List<int> triggerIds;
}

public class CardPartConfig
{
    private readonly string _path = "config/cardPart";
    private List<CardPartInfo> _list = null;
    public void init()
    {
        this._list = JsonMgr.Instance.readObject<List<CardPartInfo>>(this._path);
    }

    public List<CardPartInfo> getCartPart() {
        return this._list;
    }

    public CardPartInfo getCartPartId(int id) { 
        return this._list.Find(x => x.basePartId == id);
    }
}
