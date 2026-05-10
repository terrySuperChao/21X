public interface ITriggerHandle
{
    public TriggerEvent getTrigger();

    public bool initPokerBeforeHandle(ITriggerHandlePara para);

    public bool dealPokerBeforeHandle(ITriggerHandlePara para);

    public bool dealPokerAfterHandle(ITriggerHandlePara para);

    public bool stopPokerAfterHandle(ITriggerHandlePara para);

    public bool settlementBeforeHandle(ITriggerHandlePara para);

    public bool transformAttributeHandle(ITriggerHandlePara para);

    public bool roundAttackBeforeHandle(ITriggerHandlePara para);

    public bool normalAttackAfterHandle(ITriggerHandlePara para);

    public bool magicAttackAfterHandle(ITriggerHandlePara para);

    public bool roundAttackAfterHandle(ITriggerHandlePara para);

    public bool roundOtherHandle(ITriggerHandlePara para);
}
