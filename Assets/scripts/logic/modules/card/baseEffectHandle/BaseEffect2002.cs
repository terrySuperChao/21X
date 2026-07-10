//获得 2/4 点真实伤害
public class BaseEffect2002 : BaseEffectHandleObject
{
    private readonly int _id = GameCardConst.baseEffectId2002;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        GameBloodMgr.Instance.handle(para,addValue);
    }
}
