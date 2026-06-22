//下次普通攻击无视对手护甲；不可叠加，普通攻击后清空。

public class AdvancedEffect3003 : BaseEffectHandleObject
{
    private readonly int _id = AdvancedEffectHandleMgr.advancedEffectId3003;
    protected override int _getId()
    {
        return this._id;
    }
    protected override void _handle(ITriggerHandlePara para)
    {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        data.setState(1);
    }
}
