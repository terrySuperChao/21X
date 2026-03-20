using Pb;
using System;
using System.Collections.Generic;
using UnityEngine;
public class ShopDataMgr : Singleton<ShopDataMgr>
{
    private Shop _shop;
    public Shop newShop() {
        Shop shop = new Shop();
        return shop;
    }

    public void deserialized(GameData data) {
        this._shop = data.Shop;
    }

    public void serialized(GameData data) {

    }

    private void newShopGoods() {
        this._shop.Goods.Clear();
        this._shop.Purchased.Clear();

        List<ShopInfo> shopList = GameStaticConfigMgr.Instance.getShopConfig().getShop();
        List<ShopInfo> list = new List<ShopInfo>();
        list.AddRange(shopList);

        for (int i = 0; i < 6; i++)
        {
            Debug.Log("index==>>>" + list.Count);
            int index = RandomMgr.Instance.getRangeInt(0, list.Count);
            Debug.Log("index==>>>" + index + "===>>>" + list.Count);

            this._shop.Goods.Add(list[index].id);
            list.RemoveRange(index, 1);
            if (list.Count == 0) break;
        }
    }

    private void addBurstPurchased() {
        //爆牌一半售罄
        int point = BarrierDataMgr.Instance.getFinalPoint();
        if (point > 21)
        {
            for (int i = 0; i < this._shop.Goods.Count; i++)
            {
                int index = RandomMgr.Instance.getRangeInt(0, 2);
                if (index > 0)
                {
                    this._shop.Purchased.Add(this._shop.Goods[i]);
                }
            }
        }
    }

    //入口
    public void initEntry() {
        this.newShopGoods();
        this.addBurstPurchased();
    }

    public List<int> getGoods() { 
        List<int> list = new List<int>();
        list.AddRange(this._shop.Goods);
        return list;
    }
    
    public bool isPurchased(int id) {
        for (int i = 0; i < this._shop.Purchased.Count; i++) {
            if (this._shop.Purchased[i] == id) { 
                return true;
            }
        }
        return false;
    }

    public bool purchaseGoods(int id) {
        if (this.isPurchased(id)) {
            return false;
        }

        bool isExist = false;
        for (int i = 0; i < this._shop.Goods.Count; i++) {
            if (this._shop.Goods[i] == id) { 
                isExist = true;
                break;
            }
        }

        if (!isExist) {
            return false;
        }

        this._shop.Purchased.Add(id);

        return true;
    }

    public void refreshShop() {
        this.newShopGoods();
    }
}
