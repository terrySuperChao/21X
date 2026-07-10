//转化花色 == 黑桃
public class TriggerEffect1001 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1001;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _postSuitAttributeConversionHandle(ITriggerHandlePara para)
    {
        return para.getPokerSuit() == PokerSuit.spade;
    }
}
