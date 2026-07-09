//受到攻击时反弹 %s 点伤害
public class BaseEffect2012 : BaseEffectHandleObject
{
    private readonly int _id = GameCardConst.baseEffectId2012;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.reflectDMG);
        if (!data.isState())
        {
            data.setState(1);
            baseEffectValue.setValue(addValue);
        }
        else {
            baseEffectValue.addValue(addValue);
        }
    }
}
