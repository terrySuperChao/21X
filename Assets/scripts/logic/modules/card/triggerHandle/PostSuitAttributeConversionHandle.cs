using System.Collections.Generic;
//属性转换
public class PostSuitAttributeConversionHandle : TriggerHandleObject
{
    private Dictionary<string, PokerSuit> _suitDic =  new Dictionary<string, PokerSuit>{
        {"方块", PokerSuit.diamond},
        {"红桃", PokerSuit.heart},
        {"黑桃", PokerSuit.spade},
        {"梅花", PokerSuit.club},
    };

    private bool _isFirstFlag = false;
    private Dictionary<string, PokerSuit> _dic = new Dictionary<string, PokerSuit>{
        {"防御", PokerSuit.diamond},
        {"治疗", PokerSuit.heart},
        {"攻击", PokerSuit.spade},
        {"魔法", PokerSuit.club},
    };
    
    protected override bool _preActionHandle(ITriggerHandlePara para)
    {
        this._isFirstFlag = true;
        return base._preActionHandle(para);
    }

    protected override bool _postSuitAttributeConversionHandle(ITriggerHandlePara para)
    {
        return this.suitHandle(para) || this.conversionRatioHandle(para);
    }

    private bool suitHandle(ITriggerHandlePara para) {
        UnityEngine.Debug.Log("transform attribute handle");
        string keystr = "";
        string logic = para.getAssembleCard().getTrigger().getLogic();
        foreach (var key in this._suitDic.Keys)
        {
            if (logic.IndexOf(key) > -1)
            {
                keystr = key;
                break;
            }
        }

        if (this._suitDic.ContainsKey(keystr) &&
            this._suitDic[keystr] == para.getPokerSuit())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool conversionRatioHandle(ITriggerHandlePara para) {
        if (!this._isFirstFlag) return false;

        string keystr = "";
        string remainStr = "";
        string logic = para.getAssembleCard().getTrigger().getLogic();
        foreach (var key in this._dic.Keys)
        {
            string str = string.Format("本回合属性转化{0}占比", key);
            if (logic.IndexOf(str) > -1)
            {
                keystr = key;
                remainStr = logic.Replace(str, "");
                break;
            }
        }

        if (!this._dic.ContainsKey(keystr))
        {
            return false;
        }

        List<IPoker> pokers = FightPokerMgr.Instance.getUserHandPoker(para.getAttackUser());
        if (pokers == null ||pokers.Count == 0)
        {
            return false;
        }

        //设置标志位
        this._isFirstFlag = true;

        int mol = 0;//分子
        int denom = 0;//分母
        for (int i = 0; i < pokers.Count; i++)
        {
            if (this._dic[keystr] == pokers[i].getSuit())
            {
                mol += pokers[i].getRank();
            }
            denom += pokers[i].getRank();
        }
        if (denom == 0) return false;

        float finalRatio = 100.0f * mol / denom;
        return this.compareLogic(remainStr, finalRatio);
    }
}
