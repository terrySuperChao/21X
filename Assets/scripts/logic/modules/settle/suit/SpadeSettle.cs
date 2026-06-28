public class SpadeSettle : SuitSettle
{
    protected override float _getFinalValue(IUser attackUser, IUser defenseUser, float value) {
        return GameAttackMgr.Instance.handle(attackUser, defenseUser, value, false);
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.spade; 
    }
}