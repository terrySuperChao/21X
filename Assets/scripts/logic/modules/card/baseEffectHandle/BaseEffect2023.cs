//接下来的2回合每回合回复 %s 点生命值
public class BaseEffect2023 : BaseEffectHandleObject
{
    private readonly int _initValue = 1;
    private readonly int _stepValue = 1;
    private readonly int _maxValue = 2;
    private readonly int _id = GameCardConst.baseEffectId2023;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue1 = data.getBaseEffectValue(BaseEffectType.addLevel);
        IBaseEffectValue baseEffectValue2 = data.getBaseEffectValue(BaseEffectType.healOverTime);
        data.setState(1);
        baseEffectValue1.setValue(this._initValue);
        baseEffectValue1.setMaxValue(this._maxValue);
        baseEffectValue2.setValue(addValue);
    }

    protected override void _effect(IBaseEffectHandlePara para)
    {
        if (para.getEffectType() != AdvancedEffectType.roundStartAddBlood)
        {
            return;
        }

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        if (!data.isState()) {
            return;
        }

        IBaseEffectValue baseEffectValue1 = data.getBaseEffectValue(BaseEffectType.addLevel);
        IBaseEffectValue baseEffectValue2 = data.getBaseEffectValue(BaseEffectType.healOverTime);
        if (baseEffectValue1.getValue() <= baseEffectValue1.getMaxValue()) {
            baseEffectValue1.addValue(this._stepValue);
            GameBloodMgr.Instance.handle(para.getAttackUser(), baseEffectValue2.getValue());
        }
    }
}
