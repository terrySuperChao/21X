public class SpadeSettle : SuitSettle
{
    protected override float _getFinalValue(IUser attackUser, float value) {
        return attackUser.addAttack(value);
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.spade; 
    }
}