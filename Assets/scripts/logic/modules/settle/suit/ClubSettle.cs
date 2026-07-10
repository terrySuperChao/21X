public class ClubSettle : SuitSettle
{
    protected override float _getFinalValue(ITriggerHandlePara para, float value,out float outValue) {
        return GameMagicMgr.Instance.handle(para.getAttackUser(), value,out outValue); 
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.club; 
    }

    protected override void _suitSettle(ITriggerHandlePara para, IHandPokerSuit handPoker, float addValue)
    {
        GameMagicMgr.Instance.execAdvancedEffectHandle(para, addValue);
    }
}