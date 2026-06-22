//下一次技能释放后返还30%消耗的法力值，不可叠加，触发后清空
public class AdvancedEffect3033 : BaseEffectHandleObject
{
    private readonly float _initValue = 0.3f;
    private readonly int _id = AdvancedEffectHandleMgr.advancedEffectId3033;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        if (data.isState())
        {
            return;
        }

        data.setState(1);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.internalValue);
        baseEffectValue.setValue(this._initValue);
    }

    protected override void _effect(IBaseEffectHandlePara para)
    {
        if (para.getEffectType() != AdvancedEffectType.releaseMagic)
        {
            return;
        }

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        if (!data.isState())
        {
            return;
        }
        data.setState(0);

        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.internalValue);
        float addValue = para.getAttackUser().getMaxMagic() * baseEffectValue.getValue();
        GameMagicMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), addValue);       
    }
}
