public interface ICardHandle
{
    public void addNewCardAfterHandle(ICardHandlePara para);

    public void handPokerAfterHandle(ICardHandlePara para);

    public void dealPokerAfterHandle(ICardHandlePara para);

    public void roundBeginHandle(ICardHandlePara para);

    public void roundAddValueBeforeHandle(ICardHandlePara para);

    public void roundAddValueHandle(ICardHandlePara para);

    public void roundAddMagicHandle(ICardHandlePara para);

    public void roundSpecialAttrHandle(ICardHandlePara para);

    public void roundAttackBeforeHandle(ICardHandlePara para);

    public void roundAttackHandle(ICardHandlePara para);

    public void roundMagicAttackHandle(ICardHandlePara para);

    public void roundSubDefenseHandle(ICardHandlePara para);

    public void roundSubBloodHandle(ICardHandlePara para);

    public void roundAttackAfterHandle(ICardHandlePara para);

    public void roundEndHandle(ICardHandlePara para);
}
