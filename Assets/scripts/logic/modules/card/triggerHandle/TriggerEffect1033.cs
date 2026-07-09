//单次获得法力值>10
public class TriggerEffect1033 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1033;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _customEventHandle(ITriggerHandlePara para)
    {
        string compareStr = para.getAssembleCard().getTrigger().getLogic();
        float number = para.getTemporaryValue();
        return this.compareLogic(compareStr, number);
    }
}
