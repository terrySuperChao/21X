//转化花色 == 梅花
public class TriggerEffect1031 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1031;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _postSuitAttributeConversionHandle(ITriggerHandlePara para)
    {
        return para.getPokerSuit() == PokerSuit.club;
    }
}
