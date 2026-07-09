//下次普通攻击后，保留50%的攻击力；不可叠加，普通攻击后清空。

public class AdvancedEffect3002 : BaseEffectHandleObject
{
    private readonly float _initValue = 0.5f;
    private readonly int _id = GameCardConst.advancedEffectId3002;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.retainATK);
        if (data.isState())
        {
            return;
        }

        data.setState(1);
        baseEffectValue.setValue(this._initValue);
    }
}
