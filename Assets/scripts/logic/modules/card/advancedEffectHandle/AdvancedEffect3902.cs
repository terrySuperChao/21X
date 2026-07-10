//额外获得8点攻击力，同时造成8点真实伤害。
public class AdvancedEffect3902 : BaseEffectHandleObject
{
    private readonly int _initAttack = 8;
    private readonly int _initHurt = 8;
    private readonly int _id = GameCardConst.advancedEffectId3902;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        GameAttackMgr.Instance.handle(para, this._initAttack);
        GameBloodMgr.Instance.handle(para, this._initHurt);
    }
}
