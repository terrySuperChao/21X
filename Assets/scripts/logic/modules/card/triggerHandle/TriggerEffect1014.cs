//牌局平局
public class TriggerEffect1014 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1014;
    private readonly int _max = -1;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _postBattleResultHandle(ITriggerHandlePara para)
    {
        return para.getGameSettlePara().getWinIndex() == this._max;
    }
}
