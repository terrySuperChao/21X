//回合结束时， MP >=当前法力值上限的50%
public class TriggerEffect1034 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1034;
    private readonly int _max = 50;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _turnEndHandle(ITriggerHandlePara para)
    {
        return para.getAttackUser().getMagic() / para.getAttackUser().getMaxMagic() * 100.0f > this._max;
    }
}
