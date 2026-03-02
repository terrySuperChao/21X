using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopView : MonoBehaviour, IBaseView
{
    public GameObject point;
    public GameObject effect;
    public GameObject shopContainer;
    public GameObject shopItem;
    public GameObject sellHPBtn;
    public GameObject selectItem;

    public void init()
    {

    }

    public void beforeShow()
    {

    }

    public void refresh()
    {

    }

    public void afterShow()
    {
        int point = BarrierDataMgr.Instance.getFinalPoint();
        int value = 0;
        string effectStr = "";
        if (point < 15)
        {
            value = 30;
            effectStr = "商店价格涨价30%";
        }
        else if (point >= 15 && point <= 19)
        {
            effectStr = "商店价格不变";
        }
        else if (point >= 20 && point <= 21)
        {
            if (BarrierDataMgr.Instance.getBlackjack() == 1)
            {
                effectStr = "点数为blackjack，品质全面提升";
            }
            else
            {
                value = -30;
                effectStr = "商店价格降价30%";
            }
        }
        else
        {
            effectStr = "点数爆牌，半数商品售罄";
        }
        this.effect.GetComponent<Text>().text = effectStr;

        if (BarrierDataMgr.Instance.getBlackjack() == 1)
        {
            this.point.GetComponent<Text>().text = "当前点数：blackjack";
        }
        else {
            this.point.GetComponent<Text>().text = "当前点数：" + point;
        }

        //商店物品
        List<int> goods = ShopDataMgr.Instance.getGoods();
        for (int i = 0; i < goods.Count; i++)
        {
            int id = goods[i];
            bool isPurchased = ShopDataMgr.Instance.isPurchased(id);
            ShopInfo shopInfo = GameStaticConfigMgr.Instance.getShopConfig().getShopId(id);
            int newPrice = (int)(shopInfo.price * (100 + value) / 100.0f);

            GameObject item = UnityEngine.Object.Instantiate(this.shopItem);
            item.SetActive(true);
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
            item.transform.SetParent(shopContainer.transform, false);

            Transform nameObject = item.transform.Find("name");
            if (nameObject != null) {
                nameObject.gameObject.GetComponent<TMP_Text>().text = shopInfo.name;
            }

            Transform buyBtnObject = item.transform.Find("buyBtn");
            buyBtnObject.gameObject.SetActive(!isPurchased);
            if (buyBtnObject != null)
            {
                buyBtnObject.gameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    this.selectItem = item;
                    this.onItemClick(id, newPrice);
                });
            }

            Transform priceObject = item.transform.Find("price");
            if (priceObject != null) { 
                if (isPurchased)
                {
                    priceObject.gameObject.GetComponent<TMP_Text>().text = "<color=black>售罄</color>";
                }
                else {
                    priceObject.gameObject.GetComponent<TMP_Text>().text = "<s><color=black>" + shopInfo.price + "  </color></s>" + "<color=green>" + newPrice + "</color>";
                }
            }
        }

        if (PlayerDataMgr.Instance.getHP() < 10) {
            this.sellHPBtn.GetComponent<Button>().interactable = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.SHOPVIEW_PURCHASE, this.purchaseHandle);
        EventDispatcher.Instance.on(GameConst.SHOPVIEW_REFRESH, this.refreshHandle);
        EventDispatcher.Instance.on(GameConst.EXIT_PAGE, this.exitPageHandle);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.SHOPVIEW_PURCHASE, this.purchaseHandle);
        EventDispatcher.Instance.off(GameConst.SHOPVIEW_REFRESH, this.refreshHandle);
        EventDispatcher.Instance.off(GameConst.EXIT_PAGE, this.exitPageHandle);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void exitPageHandle(params System.Object[] obj)
    {
        string pageName = System.Enum.GetName(typeof(PageIndex), GameDataMgr.Instance.getPageIndex());
        UIMgr.Instance.showView(pageName);
    }

    public void purchaseHandle(params System.Object[] obj)
    {
        GameObject item = this.selectItem;
        Transform buyBtnObject = item.transform.Find("buyBtn");
        if (buyBtnObject != null)
        {
            buyBtnObject.gameObject.SetActive(false);
        }

        Transform priceObject = item.transform.Find("price");
        if (priceObject != null)
        {
            priceObject.gameObject.GetComponent<TMP_Text>().text = "<color=black>售罄</color>";
        }
    }

    public void refreshHandle(params System.Object[] obj)
    {
        //商店物品
        List<int> goods = ShopDataMgr.Instance.getGoods();
        for (int i = 0; i < goods.Count; i++)
        {
            int id = goods[i];
            ShopInfo shopInfo = GameStaticConfigMgr.Instance.getShopConfig().getShopId(id);
 
            GameObject item = shopContainer.transform.GetChild(i).gameObject;
            item.SetActive(true);
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
            item.transform.SetParent(shopContainer.transform, false);

            Transform nameObject = item.transform.Find("name");
            if (nameObject != null)
            {
                nameObject.gameObject.GetComponent<TMP_Text>().text = shopInfo.name;
            }

            Transform buyBtnObject = item.transform.Find("buyBtn");
            buyBtnObject.gameObject.SetActive(true);
            if (buyBtnObject != null)
            {
                buyBtnObject.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                buyBtnObject.gameObject.GetComponent<Button>().onClick.AddListener(() =>{
                    this.selectItem = item;
                    this.onItemClick(id, shopInfo.price);
                });
            }

            Transform priceObject = item.transform.Find("price");
            if (priceObject != null)
            {
                priceObject.gameObject.GetComponent<TMP_Text>().text = "<color=black>" + shopInfo.price + "  </color>";
            }
        }
        
    }

    public void onRefreshClick()
    {
        GameReqMgr.Instance.requestRefreshShop();
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void onSellHPClick()
    {
        GameReqMgr.Instance.requestSellHP();
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void onExitClick()
    {
        GameReqMgr.Instance.requestExitPage();
        GameMessage.Instance.setHandleMessageComplete();
    }
    public void onItemClick(int id,int newPrice) { 
        GameReqMgr.Instance.requestPurchaseGoods(id, newPrice);
        GameMessage.Instance.setHandleMessageComplete();
    }
}
