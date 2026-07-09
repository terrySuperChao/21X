//基础零件主数值提高30%；敌人每损失10%生命值，额外提高10%，最高提高到130%。
public class AdvancedEffect3001 : BaseEffectHandleObject
{
    private readonly float _initValue = 0.3f;
    private readonly float _maxValue = 1.3f;
    private readonly float _stepValue = 0.1f;
    private readonly float _logicValue = 0.1f;
    private readonly int _id = GameCardConst.advancedEffectId3001;
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
        if (data.isState()) {
            return;
        }

        data.setState(1);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.baseDataUp);
        baseEffectValue.setMaxValue(this._maxValue);
        baseEffectValue.setValue(this._initValue);
    }

    protected override void _effect(IBaseEffectHandlePara para) {
        if (para.getEffectType() != AdvancedEffectType.enemyLessBlood) {
            return;
        }

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        if (!data.isState()) {
            return;
        }

        if (para.getDefenseUser().getMaxBlood() * this._logicValue > para.getExtralValue())
        {
            IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.baseDataUp);
            baseEffectValue.addValue(this._stepValue);
        }     
    }
}
