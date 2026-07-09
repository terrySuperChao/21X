//基础零件主数值提高50%
public class AdvancedEffect3801 : BaseEffectHandleObject
{
    private readonly float _initValue = 0.5f;
    private readonly int _id = GameCardConst.advancedEffectId3801;
    protected override int _getId()
    {
        return this._id;
    }

    protected override float _getAdvancedValue(ITriggerHandlePara para)
    {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.baseDataUp);
        return baseEffectValue.getValue();
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        if (data.isState())
        {
            return;
        }

        data.setState(1);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.baseDataUp);
        baseEffectValue.setValue(this._initValue);
    }
}