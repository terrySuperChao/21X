//获得当前护甲 %s% 的临时护甲
public class BaseEffect2015 : BaseEffectHandleObject
{
    private readonly int _id = GameCardConst.baseEffectId2015;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        float defenseValue = para.getAttackUser().getDefense() * addValue;
        if (defenseValue <= 0) return;

        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.temporaryArmor);
        if (!data.isState())
        {
            data.setState(1);
            baseEffectValue.setValue(defenseValue);
        }
        else {
            baseEffectValue.addValue(defenseValue);
        }
        GameDefenseMgr.Instance.refreshTemporaryArmor(para, defenseValue);
    }
}
