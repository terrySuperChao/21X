public interface ISuitSettle
{
    public void settle(ICardHandlePara handlePara, IPoker poker,int baseValue);
    public ISuitSettle setNextSuitSettle(ISuitSettle nextSuitSettle);
}