//本回合属性转化红桃点数占比>=50%
using System.Collections.Generic;

public class TriggerEffect1024 : TriggerHandleObject
{
    private bool _isFirstFlag = false;
    private readonly int _max = 50;
    private readonly int _id = GameCardConst.TriggerEffectId1024;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _preActionHandle(ITriggerHandlePara para)
    {
        this._isFirstFlag = true;
        return base._preActionHandle(para);
    }

    protected override bool _postSuitAttributeConversionHandle(ITriggerHandlePara para)
    {
        if (!this._isFirstFlag) return false;

        List<IPoker> pokers = FightPokerMgr.Instance.getUserHandPoker(para.getAttackUser());
        if (pokers == null || pokers.Count == 0)
        {
            return false;
        }

        //设置标志位
        this._isFirstFlag = true;

        int mol = 0;//分子
        int denom = 0;//分母
        for (int i = 0; i < pokers.Count; i++)
        {
            if (pokers[i].getSuit() == PokerSuit.heart)
            {
                mol += pokers[i].getRank();
            }
            denom += pokers[i].getRank();
        }
        if (denom == 0) return false;

        float finalRatio = 100.0f * mol / denom;
        return finalRatio >= this._max;
    }
}
