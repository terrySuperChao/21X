//获胜且手牌包含 A
using System.Collections.Generic;

public class TriggerEffect1051 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1051;
    private readonly int _max = 14;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _postBattleResultHandle(ITriggerHandlePara para)
    {
        //获胜的判断
        if (para.getGameSettlePara().getWinIndex() == -1)
        {
            return false;
        }

        if (para.getGameSettlePara().getWinIndex() == 0 && !para.getAttackUser().isNpc())
        {
            return false;
        }

        if (para.getGameSettlePara().getWinIndex() == 1 && para.getAttackUser().isNpc())
        {
            return false;
        }

        List<IPoker> pokers = FightPokerMgr.Instance.getUserHandPoker(para.getAttackUser());
        int index = pokers.FindIndex(poker => poker.getRank() == this._max);
        return index != -1;
    }
}
