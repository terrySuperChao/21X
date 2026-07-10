//额外回复4点生命值和8点法力值。
public class AdvancedEffect3904 : BaseEffectHandleObject
{
    private readonly int _initBlood = 4;
    private readonly int _initMagic = 8;
    private readonly int _id = GameCardConst.advancedEffectId3904;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        GameBloodMgr.Instance.handle(para.getAttackUser(), this._initBlood);
        GameMagicMgr.Instance.handle(para, this._initMagic);
    }
}
