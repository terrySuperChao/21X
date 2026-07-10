//停牌总点数在 [17, 21] 之间，且至少有1张方块
using System.Collections.Generic;

public class TriggerEffect1013 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1013;
    private readonly int _min = 17;
    private readonly int _max = 21;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _postStandOrFinalScoreHandle(ITriggerHandlePara para)
    {  
        FightDealType type = para.getAttackUser().isNpc() ? FightDealType.npc : FightDealType.player;
        List<IPoker> pokers = FightDataMgr.Instance.getPokers(type);
        int index = pokers.FindIndex(poker => poker.getSuit() == PokerSuit.diamond);
        if (index == -1)
        {
            return false;
        }

        float point = (float)FightPokerMgr.Instance.getUserHandPokerPoint(para.getAttackUser(), false);
        if (this._min <= point && this._max >= point)
        {
            return true;
        }

        return false;
    }
}
