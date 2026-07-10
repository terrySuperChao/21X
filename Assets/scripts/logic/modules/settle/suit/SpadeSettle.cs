public class SpadeSettle : SuitSettle
{
    protected override float _getFinalValue(ITriggerHandlePara para, float value, out float outValue) {
        return GameAttackMgr.Instance.handle(para.getAttackUser(), value,out outValue);
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.spade; 
    }

    protected override void _suitSettle(ITriggerHandlePara para, IHandPokerSuit handPoker,float addValue)
    {
        GameRunTimeMgr.Instance.runTimeCountAttack(para, addValue);
    }
}