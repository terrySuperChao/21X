//本局技能释放所需 MP 减少 %s
public class BaseEffect2035 : BaseEffectHandleObject
{
    private readonly int _id = GameCardConst.baseEffectId2035;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        GameMagicMgr.Instance.addmaxMagicHandle(para.getAttackUser(), -addValue);
    }
}
