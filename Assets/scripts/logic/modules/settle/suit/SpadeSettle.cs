public class SpadeSettle : SuitSettle
{
    override
    protected float _getFinalValue(IUser attackUser, float value) {
        return attackUser.addAttack(value);
    }

    override
    protected bool _matchSuit(int suit) { 
        return suit == (int)PokerSuit.spade; 
    }
}