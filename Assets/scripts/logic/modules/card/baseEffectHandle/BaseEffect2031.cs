//获得 %s 点法力值
public class BaseEffect2031 : BaseEffectHandleObject
{
    private readonly int _id = GameCardConst.baseEffectId2031;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        GameMagicMgr.Instance.handle(para.getAttackUser(),para.getDefenseUser(), addValue);   
    }
}
