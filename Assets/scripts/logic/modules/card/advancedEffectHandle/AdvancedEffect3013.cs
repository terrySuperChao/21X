//获得8点临时护甲，本回合结束时，当前所有剩余临时护甲转化为普通护甲。
public class AdvancedEffect3013 : BaseEffectHandleObject
{
    private readonly int _initValue = 8;
    private readonly int _id = GameCardConst.advancedEffectId3013;
    private ITriggerHandlePara _paras = new TriggerHandleParaObject();
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.temporaryArmor);
        if (!data.isState())
        {
            data.setState(1);
            baseEffectValue.setValue(this._initValue);
        }
        else {
            baseEffectValue.addValue(this._initValue);
        }
        GameDefenseMgr.Instance.refreshTemporaryArmor(para, this._initValue);
    }

    protected override void _effect(IBaseEffectHandlePara para)
    {
        if (para.getEffectType() != AdvancedEffectType.transformCommonDefense)
        {
            return;
        }

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        if (!data.isState())
        {
            return;
        }

        this._paras.setAttackUser(para.getAttackUser());
        this._paras.setDefenseUser(para.getDefenseUser());
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.temporaryArmor);
        GameDefenseMgr.Instance.handle(this._paras,baseEffectValue.getValue());
    }
}
