public class DiamondSettle: SuitSettle
{
    override
    protected float _getFinalValue(IUser attackUser, float value) {
        return attackUser.addDefense(value);
    }

    override
    protected bool _matchSuit(int suit) { 
        return suit == (int)PokerSuit.diamond; 
    }

    override
    protected float _getMult() { return 0.5f; }
}