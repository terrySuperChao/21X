public class ClubSettle : SuitSettle
{
    protected override float _getFinalValue(ITriggerHandlePara para, float value) {
        return GameMagicMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), value,false); 
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.club; 
    }
}