//我方释放主技能后
public class TriggerEffect1032 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1032;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _postMainSkillHandle(ITriggerHandlePara para)
    {
        return true;
    }
}
