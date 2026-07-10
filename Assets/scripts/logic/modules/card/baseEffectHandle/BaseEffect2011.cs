//获得 %s 点护甲
public class BaseEffect2011 : BaseEffectHandleObject
{
    private readonly int _id = GameCardConst.baseEffectId2011;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        GameDefenseMgr.Instance.handle(para, addValue);
    }
}
