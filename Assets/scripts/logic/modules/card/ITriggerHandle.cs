public interface ITriggerHandle
{
    public int getTrigger();
    //�����¿���
    public void addNewCardAfterHandle(ICardHandlePara para);
    //����
    public void handPokerAfterHandle(ICardHandlePara para);

    //Ҫ��
    public void dealPokerAfterHandle(ICardHandlePara para);
    //�غ�ǰ
    public void roundBeginHandle(ICardHandlePara para);
    //�غ�����ֵǰ
    public void roundAddValueBeforeHandle(ICardHandlePara para);
    //�غ�����ֵ
    public void roundAddValueHandle(ICardHandlePara para);
    //�غ�����ħ��ֵ
    public void roundAddMagicHandle(ICardHandlePara para);
    //��������
    public void roundSpecialAttrHandle(ICardHandlePara para);
    //�غϹ���ǰ
    public void roundAttackBeforeHandle(ICardHandlePara para);
    //����
    public void roundAttackHandle(ICardHandlePara para);
    //ħ������
    public void roundMagicAttackHandle(ICardHandlePara para);
    //�ۻ���
    public void roundSubDefenseHandle(ICardHandlePara para);
    //��Ѫ
    public void roundSubBloodHandle(ICardHandlePara para);
    //�غϹ�����
    public void roundAttackAfterHandle(ICardHandlePara para);
    //�غϽ���
    public void roundEndHandle(ICardHandlePara para);
}
