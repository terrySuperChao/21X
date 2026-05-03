public interface ISuitSettle
{
    public void settle(ITriggerHandlePara para, IPoker poker,int baseValue);
    public ISuitSettle setNextSuitSettle(ISuitSettle nextSuitSettle);
}