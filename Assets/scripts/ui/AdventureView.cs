using System;
using UnityEngine;
using UnityEngine.UI;

public class AdventureView : MonoBehaviour, IBaseView
{
    public GameObject point;
    public GameObject effect;

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
        string effectStr = "";
        if (point < 15)
        {
            effectStr = "点数总和<15，今天有点倒霉";
        }
        else if (point >= 15 && point <= 19)
        {
            effectStr = "点数总和=15-19，一次普通的路过";
        }
        else if (point >= 20 && point <= 21)
        {
            if (BarrierDataMgr.Instance.getBlackjack() == 1)
            {
                effectStr = "点数为blackjack，运气爆棚";
            }
            else
            {
                effectStr = "今天手气不错";
            }
        }
        else
        {
            effectStr = "点数爆牌，屋漏偏逢连夜雨";
        }
        this.effect.GetComponent<Text>().text = effectStr;

        if (BarrierDataMgr.Instance.getBlackjack() == 1)
        {
            this.point.GetComponent<Text>().text = "当前点数：blackjack";
        }
        else {
            this.point.GetComponent<Text>().text = "当前点数：" + point;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        EventDispatcher.Instance.on(GameConst.ADVENTURE_FOOD, this.foodHandle);
        EventDispatcher.Instance.on(GameConst.ADVENTURE_EXIT, this.exitHandle);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.off(GameConst.ADVENTURE_FOOD, this.foodHandle);
        EventDispatcher.Instance.off(GameConst.ADVENTURE_EXIT, this.exitHandle);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void foodHandle(params System.Object[] obj)
    {
       
    }

    public void exitHandle(params System.Object[] obj)
    {
        string pageName = Enum.GetName(typeof(PageIndex), GameDataMgr.Instance.getPageIndex());
        UIMgr.Instance.showView(pageName);
    }

    public void onFoodClick()
    {
       
    }

    public void onFireClick()
    {
        
    }

    public void onRobClick() { 

    }

    public void onExitClick()
    {
        GameReqMgr.Instance.requestExitAdventure();
        GameMessage.Instance.setHandleMessageComplete();
    }
}
