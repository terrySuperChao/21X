//开始行动时，敌方当前护甲 > 0
public class TriggerEffect1003 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1003;
    private readonly int _max = 0;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _preActionHandle(ITriggerHandlePara para)
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

        return para.getDefenseUser().getDefense() > this._max;
    }
}
