public interface ISuitSettle
{
    public void settle(ITriggerHandlePara para,IHandPokerSuit handPoker);
    public ISuitSettle setNextSuitSettle(ISuitSettle nextSuitSettle);
}