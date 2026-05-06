public interface ITriggerHandle
{
    public TriggerEvent getTrigger();
 
    public bool addNewCardAfterHandle(ITriggerHandlePara para);

    public bool handPokerAfterHandle(ITriggerHandlePara para);

    public bool dealPokerAfterHandle(ITriggerHandlePara para);
    //平局
    public bool roundAbortionHandle(ITriggerHandlePara para);

    public bool roundBeginHandle(ITriggerHandlePara para);

    public bool roundAddValueBeforeHandle(ITriggerHandlePara para);

    public bool roundAddValueHandle(ITriggerHandlePara para);

    public bool roundAddMagicHandle(ITriggerHandlePara para);

    public bool roundSpecialAttrHandle(ITriggerHandlePara para);

    public bool roundAttackBeforeHandle(ITriggerHandlePara para);

    public bool roundAttackHandle(ITriggerHandlePara para);

    public bool roundMagicAttackHandle(ITriggerHandlePara para);

    public bool roundSubDefenseHandle(ITriggerHandlePara para);

    public bool roundSubBloodHandle(ITriggerHandlePara para);

    public bool roundAttackAfterHandle(ITriggerHandlePara para);

    public bool roundEndHandle(ITriggerHandlePara para);
}
