//首次触发回复 %s 点生命值，之后每次触发回复少量生命
public class BaseEffect2025 : BaseEffectHandleObject
{
    private readonly int _id = GameCardConst.baseEffectId2025;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        if (!data.isState()) {
            data.setState(1);
            float addValue = this.getAddValue(para);
            GameBloodMgr.Instance.handle(para.getAttackUser(), addValue);
        }
        else
        {
            float addValue = this.getAddValue(para,1);
            GameBloodMgr.Instance.handle(para.getAttackUser(), addValue);
        }
    }
}
