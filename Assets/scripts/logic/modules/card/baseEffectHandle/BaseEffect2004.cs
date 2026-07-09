//下次普通攻击额外造成 %s% 的伤害，最多叠加5层
public class BaseEffect2004 : BaseEffectHandleObject
{
    private readonly int _initValue = 1;
    private readonly int _stepValue = 1;
    private readonly int _maxValue = 5;
    private readonly int _id = GameCardConst.baseEffectId2004;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue1 = data.getBaseEffectValue(BaseEffectType.addLevel);
        IBaseEffectValue baseEffectValue2 = data.getBaseEffectValue(BaseEffectType.multATK);
        if (!data.isState())
        {
            data.setState(1);
            baseEffectValue1.setMaxValue(this._maxValue);
            baseEffectValue1.setValue(this._initValue);
            baseEffectValue2.setValue(addValue);
        }
        else {
            if (baseEffectValue1.getValue() < baseEffectValue1.getMaxValue()) {
                baseEffectValue1.addValue(this._stepValue);
                baseEffectValue2.addValue(addValue);
            }
        }
    }
}
