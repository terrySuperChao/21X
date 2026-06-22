//基础零件主数值提高30%；自身每有5点护甲，额外提高10%，最高提高到130%。
public class AdvancedEffect3011 : BaseEffectHandleObject
{
    private readonly float _initValue = 0.3f;
    private readonly float _maxValue = 1.3f;
    private readonly float _stepValue = 0.1f;
    private readonly float _logicValue = 5f;
    private readonly int _id = AdvancedEffectHandleMgr.advancedEffectId3011;
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
        if (para.getEffectType() != AdvancedEffectType.addDefense)
        {
            return;
        }

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        if (!data.isState()) {
            return;
        }

        IUser attackUser = para.getAttackUser();
        attackUser.getExtraInfo().setRtAddDefenseValue(para.getExtralValue());

        float defense  = attackUser.getExtraInfo().getRtAddDefenseValue(); ;
        int number = (int)(defense / this._logicValue);

        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.baseDataUp);
        baseEffectValue.addValue(this._stepValue * number);

        attackUser.getExtraInfo().setRtAddDefenseValue(-number * this._logicValue);
    }
}
