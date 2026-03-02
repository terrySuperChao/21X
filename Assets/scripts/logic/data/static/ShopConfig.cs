using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ShopInfo
{
    public int id;
    public string name;
    public int type;
    public int quality;
    public int price;
    public string image;
    public string desc;
}

public class ShopConfig
{
    private readonly string _path = "config/shop";
    private List<ShopInfo> _shopList = null;
    public void init()
    {
        this._shopList = JsonMgr.Instance.readObject<List<ShopInfo>>(this._path);
    }

    public List<ShopInfo> getShop() {
        return this._shopList;
    }
}
