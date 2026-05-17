public class ClubSettle : SuitSettle
{
    override
    protected float _getFinalValue(IUser attackUser, float value) {
        return attackUser.addMagic(value); 
    }

    override
    protected bool _matchSuit(int suit) { 
        return suit == (int)PokerSuit.club; 
    }

    override
    protected void _settle(ITriggerHandlePara para, IPoker poker, int baseValue)
    {
        para.getAttackUser().getExtraInfo().setRtMagicValue(baseValue);
    }
}