using System.Collections.Generic;

//本回合属性转化治疗占比>=50%
public class TransformAttributeRatioHandle : TriggerHandleObject
{
    private bool _isFirstFlag = false;
    private Dictionary<string, PokerSuit> _dic =  new Dictionary<string, PokerSuit>{
        {"防御", PokerSuit.diamond},
        {"治疗", PokerSuit.heart},
        {"攻击", PokerSuit.spade},
        {"魔法", PokerSuit.club},
    };

    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.transformAttribute;
    }

    protected override bool _settlementBeforeHandle(ITriggerHandlePara para) {
        this._isFirstFlag = true;
        return base._settlementBeforeHandle(para);
    }

    protected override bool _transformAttributeHandle(ITriggerHandlePara para)
    {
        if (!this._isFirstFlag) return false;

        string keystr = "";
        string remainStr = "";
        string logic = para.getAssembleCard().getTrigger().getLogic();
        foreach (var key in this._dic.Keys)
        {
            string str = string.Format("本回合属性转化{0}占比", key);
            if (logic.IndexOf(str) > -1) {
                keystr = key;
                remainStr = logic.Replace(str, "");
                break;
            }
        }

        if (!this._dic.ContainsKey(keystr))
        {
            return false;
        }
        
        List<IPoker> pokers = FightPokerMgr.Instance.getUsetHandPoker(para.getUser());
        if (pokers == null ||
            pokers.Count == 0 ||
            pokers[pokers.Count - 1] != para.getPoker()) {
            return false;
        }

        //设置标志位
        this._isFirstFlag = true;

        int mol = 0;//分子
        int denom = 0;//分母
        for (int i = 0; i < pokers.Count; i++) {
            if (this._dic[keystr] == (PokerSuit)pokers[i].getSuit()) {
                mol += pokers[i].getRank();
            }
            denom += pokers[i].getRank();
        }
        if (denom == 0) return false;

        float finalRatio = 100.0f * mol / denom;
        return this.compareLogic(remainStr, finalRatio);
    }
}
