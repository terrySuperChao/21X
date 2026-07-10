//单次造成伤害 >= 15
public class TriggerEffect1023 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1023;
    private readonly int _max = 15;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _customEventHandle(ITriggerHandlePara para)
    {
        string compareStr = para.getAssembleCard().getTrigger().getLogic();
        float number = para.getTemporaryValue();
        return number >= this._max;
    }
}
