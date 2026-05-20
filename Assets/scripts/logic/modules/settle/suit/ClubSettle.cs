public class ClubSettle : SuitSettle
{
    protected override float _getFinalValue(IUser attackUser, float value) {
        return attackUser.addMagic(value); 
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.club; 
    }

    protected override void _settle(ITriggerHandlePara para, IPoker poker, int baseValue)
    {
        para.getAttackUser().getExtraInfo().setRtMagicValue(baseValue);
    }
}