//下一次玩家获得的法力值*2，且额外对敌人造成获得法力值50%的真实伤害，不可叠加，触发后清空
public class AdvancedEffect3032 : BaseEffectHandleObject
{
    private readonly float _initValue = 2;
    private readonly float _initHurt = 0.5f;
    private readonly int _id = GameCardConst.advancedEffectId3032;
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
        IBaseEffectValue baseEffectValue1 = data.getBaseEffectValue(BaseEffectType.magicDouble);
        IBaseEffectValue baseEffectValue2 = data.getBaseEffectValue(BaseEffectType.magicHurt);
        baseEffectValue1.setValue(this._initValue);
        baseEffectValue2.setValue(this._initHurt);
    }

    protected override void _effect(IBaseEffectHandlePara para)
    {
        if (para.getEffectType() != AdvancedEffectType.addMagic)
        {
            return;
        }

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        if (!data.isState())
        {
            return;
        }

        float addValue = para.getExtralValue();
        float magicHurt = GameCardMgr.Instance.getBaseEffectValue(para.getAttackUser(),BaseEffectType.magicHurt);
        GameBloodMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), addValue * magicHurt);
        data.setState(0);
    }
}
