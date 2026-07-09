//回复 %s 点生命值
public class BaseEffect2021 : BaseEffectHandleObject
{
    private readonly int _id = GameCardConst.baseEffectId2021;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        GameBloodMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), addValue);        
    }
}
