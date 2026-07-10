//额外获得4点护甲和4点临时护甲。
public class AdvancedEffect3903 : BaseEffectHandleObject
{
    private readonly int _initDefense = 4;
    private readonly int _initTemporaryDefense = 4;
    private readonly int _id = GameCardConst.advancedEffectId3903;
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
            baseEffectValue.setValue(this._initTemporaryDefense);
        }
        else {
            baseEffectValue.addValue(this._initTemporaryDefense);
        }
        GameDefenseMgr.Instance.handle(para, this._initDefense);        
    }
}
