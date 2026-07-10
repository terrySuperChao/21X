//获胜且点数 >= 20
public class TriggerEffect1002 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1002;
    private readonly int _max = 20;
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

        float point = (float)FightPokerMgr.Instance.getUserHandPokerPoint(para.getAttackUser(), false);
        return point >= this._max;
    }
}
