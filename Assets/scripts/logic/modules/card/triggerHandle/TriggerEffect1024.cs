//本回合属性转化红桃点数占比>=50%
using System.Collections.Generic;

public class TriggerEffect1024 : TriggerHandleObject
{
    private readonly int _max = 50;
    private readonly int _id = GameCardConst.TriggerEffectId1024;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _preActionHandle(ITriggerHandlePara para)
    {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        data.setState(0);
        return base._preActionHandle(para);
    }

    protected override bool _postSuitAttributeConversionHandle(ITriggerHandlePara para)
    {
        List<IPoker> pokers = FightPokerMgr.Instance.getUserHandPoker(para.getAttackUser());
        if (pokers == null || pokers.Count == 0)
        {
            return false;
        }

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        if (data.isState()) {
            return false;
        }
        data.setState(1);

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
