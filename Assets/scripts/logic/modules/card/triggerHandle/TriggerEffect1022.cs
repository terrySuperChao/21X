//本回合受到伤害，且回合结束时自身血量 < 60%
public class TriggerEffect1022 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1022;
    private readonly int _max = 60;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _turnEndHandle(ITriggerHandlePara para)
    {
        return para.getAttackUser().getBlood() / para.getAttackUser().getMaxBlood() * 100.0f < this._max;
    }
}
