//额外获得当前法力值的 %s% 的法力值
public class BaseEffect2034 : BaseEffectHandleObject
{
    private readonly int _id = GameCardConst.baseEffectId2034;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        float magicValue = addValue * para.getAttackUser().getMagic();
        GameMagicMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), magicValue);
    }
}
