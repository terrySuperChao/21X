//回复8点生命，本回合溢出的治疗量转化为攻击力。
public class AdvancedEffect3022 : BaseEffectHandleObject
{
    private readonly int _initValue = 8;
    private readonly int _id = AdvancedEffectHandleMgr.advancedEffectId3022;
    protected override int _getId()
    {
        return this._id;
    }
    protected override void _handle(ITriggerHandlePara para)
    {
        GameBloodMgr.Instance.handle(para.getAttackUser(), this._initValue);

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.internalValue);
        float addValue = baseEffectValue.getValue();
        if (addValue > 0) {
            GameAttackMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), addValue);
            baseEffectValue.clearValue();
        }
    }

    protected override void _effect(IBaseEffectHandlePara para)
    {
        if (para.getEffectType() != AdvancedEffectType.overflowBloodValue) {
            return;
        }

        if (para.getExtralValue() <= 0) {
            return;
        }

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.internalValue);
        baseEffectValue.addValue(para.getExtralValue());
    }
}
