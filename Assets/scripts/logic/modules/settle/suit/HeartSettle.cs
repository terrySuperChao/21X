public class HeartSettle : SuitSettle
{
    override
    protected float _getFinalValue(IUser attackUser, float value) {
        return attackUser.addBlood(value);
    }

    override
    protected bool _matchSuit(int suit) { 
        return suit == (int)PokerSuit.heart; 
    }

    override
    protected float _getMult() { return 0.5f; }
}