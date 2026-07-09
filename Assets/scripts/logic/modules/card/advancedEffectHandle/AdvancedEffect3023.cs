//本场战斗首次受到致命伤时，不会死亡，且回复30点生命值
public class AdvancedEffect3023 : BaseEffectHandleObject
{
    private readonly float _initValue = 30;
    private readonly int _id = GameCardConst.advancedEffectId3023;
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
        if (para.getEffectType() != AdvancedEffectType.selfLessBlood)
        {
            return;
        }

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        if (!data.isState())
        {
            return;
        }

        if (para.getAttackUser().getBlood() > 0)
        {
            return;
        }

        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.internalValue);
        float addValue = baseEffectValue.getValue();
        if (addValue <= 0) {
            return;
        }
        baseEffectValue.clearValue();
        GameBloodMgr.Instance.handle(para.getAttackUser(), addValue);
    }
}
