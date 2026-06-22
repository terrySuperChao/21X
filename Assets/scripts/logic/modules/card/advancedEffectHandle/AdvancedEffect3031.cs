//基础零件主数值提高30%；当前每有10点法力，额外提高10%，最高提高到130%。
using Unity.VisualScripting;

public class AdvancedEffect3031 : BaseEffectHandleObject
{
    private readonly float _initValue = 0.3f;
    private readonly float _maxValue = 1.3f;
    private readonly float _stepValue = 0.1f;
    private readonly float _logicValue = 0.1f;
    private readonly int _id = AdvancedEffectHandleMgr.advancedEffectId3031;
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
        baseEffectValue.setMaxValue(this._maxValue);
        baseEffectValue.setValue(this._initValue);
    }

    protected override void _effect(IBaseEffectHandlePara para)
    {
        if (para.getEffectType() != AdvancedEffectType.addMagic)
        {
            return;
        }

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue1 = data.getBaseEffectValue(BaseEffectType.baseDataUp);
        IBaseEffectValue baseEffectValue2 = data.getBaseEffectValue(BaseEffectType.rtMagicValue);
        IBaseEffectValue baseEffectValue3 = data.getBaseEffectValue(BaseEffectType.rtMagicTotal);
        baseEffectValue2.addValue(para.getExtralValue());
        baseEffectValue3.addValue(para.getExtralValue());

        float magicValue = baseEffectValue2.getValue();
        int per = (int)(magicValue / this._logicValue);

        baseEffectValue1.addValue(this._stepValue * per);
        baseEffectValue2.addValue(this._logicValue * per * -1);
    }
}
