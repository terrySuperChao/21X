//转化花色 == 方块
public class TriggerEffect1011 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1011;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _postSuitAttributeConversionHandle(ITriggerHandlePara para)
    {
        return para.getPokerSuit() == PokerSuit.diamond;
    }
}
