using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RelaxView : MonoBehaviour, IBaseView
{
    public GameObject point;
    public GameObject effect;
    public GameObject addHp;
    
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
        string hpStr = "";
        if (point < 15)
        {
            effectStr = "点数总和<15，治疗效果-50%";
            hpStr = "恢复<s>20</s><color=red>10</color>点生命力";
        }
        else if (point >= 15 && point <= 19)
        {
            effectStr = "点数总和=15-19，治疗效果不变";
            hpStr = "恢复20点生命力";
        }
        else if (point >= 20 && point <= 21)
        {
            if (BarrierDataMgr.Instance.getBlackjack() == 1)
            {
                effectStr = "点数为blackjack，治疗效果+50%，额外增加生命值上限";
                hpStr = "恢复<s>20</s><color=red>40</color>点生命力,额外增加生命值上限10";
            }
            else
            {
                effectStr = "点数总和=20-21，治疗效果+50%";
                hpStr = "恢复<s>20</s><color=red>40</color>点生命力";
            }
        }
        else
        {
            effectStr = "点数爆牌，仅能获得随机治疗";
            hpStr = "仅能获得随机治疗";
        }
        this.effect.GetComponent<Text>().text = effectStr;
        this.addHp.GetComponent<TMP_Text>().text = hpStr;

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

    public void onRelaxClick()
    {
        GameReqMgr.Instance.requestRelax();
        GameMessage.Instance.setHandleMessageComplete();
    }

    public void onTrainClick()
    {

    }

    public void onExitClick() {
        GameReqMgr.Instance.requestExitPage();
        GameMessage.Instance.setHandleMessageComplete();
    }
}
