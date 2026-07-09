//基础零件主数值提高30%；本场战斗中该印记每触发一次，加成提高10%，最高提高到130%。
public class AdvancedEffect3802 : BaseEffectHandleObject
{
    private readonly float _initValue = 0.3f;
    private readonly float _maxValue = 1.3f;
    private readonly float _stepValue = 0.1f;
    private readonly int _id = GameCardConst.advancedEffectId3802;
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
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.baseDataUp);
        if (data.isState())
        {
            baseEffectValue.addValue(this._stepValue);
        }else {
            data.setState(1);
            baseEffectValue.setMaxValue(this._maxValue);
            baseEffectValue.setValue(this._initValue);
        }
    }
}
