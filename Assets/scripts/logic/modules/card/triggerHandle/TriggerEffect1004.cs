//开始行动时，敌方当前血量 < 40%
public class TriggerEffect1004 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1004;
    private readonly int _max = 40;
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

        return para.getDefenseUser().getBlood() / para.getDefenseUser().getMaxBlood() * 100.0f < this._max;
    }
}
