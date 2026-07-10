//获得4点护甲，并造成当前护甲50%的真实伤害
public class AdvancedEffect3012 : BaseEffectHandleObject
{
    private readonly int _initValue = 4;
    private readonly float _initHurt = 0.5f;
    private readonly int _id = GameCardConst.advancedEffectId3012;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        GameDefenseMgr.Instance.handle(para, this._initValue);

        float attack = para.getAttackUser().getDefense() * this._initHurt;
        GameBloodMgr.Instance.handle(para, attack);
    }
}
