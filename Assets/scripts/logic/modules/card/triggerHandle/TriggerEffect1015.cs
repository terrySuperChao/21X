//回合结束时，自身护甲值 >= 10
public class TriggerEffect1015 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1015;
    private readonly int _max = 10;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _turnEndHandle(ITriggerHandlePara para)
    {
        return para.getAttackUser().getDefense() >= this._max;
    }
}
