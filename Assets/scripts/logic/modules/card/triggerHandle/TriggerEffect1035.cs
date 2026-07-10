//本回合要牌 >= 3
using System.Collections.Generic;

public class TriggerEffect1035 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1035;
    private readonly int _max = 3;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _postCardDrawHandle(ITriggerHandlePara para)
    {
        //通过手牌确定要了几次牌
        List<IPoker> pokers = FightPokerMgr.Instance.getUserHandPoker(para.getAttackUser());
        return pokers.Count - 2 > this._max;
    }
}
