//获得当前护甲 %s% 的攻击力
public class BaseEffect2013 : BaseEffectHandleObject
{
    private readonly int _id = BaseEffectHandleMgr.baseEffectId2013;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);        
        float attackValue = para.getAttackUser().getDefense() * addValue;
        GameAttackMgr.Instance.handle(para, attackValue);
    }
}
