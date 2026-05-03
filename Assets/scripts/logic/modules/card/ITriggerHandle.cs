public interface ITriggerHandle
{
    public TriggerEvent getTrigger();
 
    public void addNewCardAfterHandle(ITriggerHandlePara para);

    public void handPokerAfterHandle(ITriggerHandlePara para);

    public void dealPokerAfterHandle(ITriggerHandlePara para);

    public void roundBeginHandle(ITriggerHandlePara para);

    public void roundAddValueBeforeHandle(ITriggerHandlePara para);

    public void roundAddValueHandle(ITriggerHandlePara para);

    public void roundAddMagicHandle(ITriggerHandlePara para);

    public void roundSpecialAttrHandle(ITriggerHandlePara para);

    public void roundAttackBeforeHandle(ITriggerHandlePara para);

    public void roundAttackHandle(ITriggerHandlePara para);

    public void roundMagicAttackHandle(ITriggerHandlePara para);

    public void roundSubDefenseHandle(ITriggerHandlePara para);

    public void roundSubBloodHandle(ITriggerHandlePara para);

    public void roundAttackAfterHandle(ITriggerHandlePara para);

    public void roundEndHandle(ITriggerHandlePara para);
}
