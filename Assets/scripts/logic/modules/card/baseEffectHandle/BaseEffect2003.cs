//削减目标 4/7 点护甲
public class BaseEffect2003 : BaseEffectHandleObject
{
    private readonly int _id = GameCardConst.baseEffectId2003;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        SwitchParaMgr.Instance.handle(para, () =>
        {
            GameDefenseMgr.Instance.handle(para, -addValue);
        }, true);
    }
}
