public class ClubSettle : SuitSettle
{
    protected override float _getFinalValue(IUser attackUser, IUser defenseUser, float value) {
        return GameMagicMgr.Instance.handle(attackUser, defenseUser,value,false); 
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.club; 
    }
}