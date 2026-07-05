//获得 3/5 点攻击力
public class BaseEffect2001 : BaseEffectHandleObject
{
    private readonly int _id = BaseEffectHandleMgr.baseEffectId2001;
    protected override int _getId() {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        GameAttackMgr.Instance.handle(para, addValue);
    }
}
