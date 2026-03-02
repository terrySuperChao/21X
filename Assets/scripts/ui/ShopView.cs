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
        List<ShopInfo> shopList = GameStaticConfigMgr.Instance.getShopConfig().getShop();
        for (int i = 0; i < shopList.Count; i++)
        {
            GameObject item = UnityEngine.Object.Instantiate(this.shopItem);
            item.SetActive(true);
            item.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
            item.transform.SetParent(shopContainer.transform, false);

            Transform name = item.transform.Find("name");
            if (name != null) {
                name.gameObject.GetComponent<TMP_Text>().text = shopList[i].name;
            }

            Transform price = item.transform.Find("price");
            if (price != null)
            {
                int newPrice = (int)(shopList[i].price * (100 + value) / 100.0f);
                price.gameObject.GetComponent<TMP_Text>().text = "<s><color=black>"+shopList[i].price+"  </color></s>" + "<color=green>"+ newPrice + "</color>";
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.RELAXVIEW_RELAX, this.relaxHandle);
        EventDispatcher.Instance.on(GameConst.EXIT_PAGE, this.exitPageHandle);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.RELAXVIEW_RELAX, this.relaxHandle);
        EventDispatcher.Instance.off(GameConst.EXIT_PAGE, this.exitPageHandle);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void exitPageHandle(params System.Object[] obj)
    {
        string pageName = Enum.GetName(typeof(PageIndex), GameDataMgr.Instance.getPageIndex());
        UIMgr.Instance.showView(pageName);
    }

    public void relaxHandle(params System.Object[] obj)
    {
        if (GameDataMgr.Instance.getPageIndex() != PageIndex.RelaxView) {
            string pageName = Enum.GetName(typeof(PageIndex), GameDataMgr.Instance.getPageIndex());
            UIMgr.Instance.showView(pageName);
        }
    }

    public void onRefreshClick()
    {
        GameReqMgr.Instance.requestRelax();
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void onSellClick()
    {

    }

    public void onExitClick()
    {
        GameReqMgr.Instance.requestExitPage();
        GameMessage.Instance.setHandleMessageComplete();
    }
}
