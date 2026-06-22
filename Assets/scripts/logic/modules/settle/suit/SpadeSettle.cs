public class SpadeSettle : SuitSettle
{
    protected override float _getFinalValue(IUser attackUser, IUser defenseUser, float value) {
        return attackUser.addAttack(value);
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.spade; 
    }
}