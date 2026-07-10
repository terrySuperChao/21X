//转化花色 == 红桃
public class TriggerEffect1021 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1021;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _postSuitAttributeConversionHandle(ITriggerHandlePara para)
    {
        return para.getPokerSuit() == PokerSuit.heart;
    }
}
