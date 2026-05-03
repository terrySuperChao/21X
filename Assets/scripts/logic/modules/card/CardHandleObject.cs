using System;

public abstract class CardHandleObject : ICardHandle
{
    //�����¿���
    public void addNewCardAfterHandle(ICardHandlePara para) {
        _addNewCardAfterHandle(para);
    }

    //���ƺ�
    public void handPokerAfterHandle(ICardHandlePara para) {
        _handPokerAfterHandle(para);
    }
    //Ҫ�ƺ�
    public void dealPokerAfterHandle(ICardHandlePara para) {
        _dealPokerAfterHandle(para);
    }

    //�غ�ǰ
    public void roundBeginHandle(ICardHandlePara para)
    {
        _roundBeginHandle(para);
    }

    //�غ�����ֵǰ
    public void roundAddValueBeforeHandle(ICardHandlePara para) {
        _roundAddValueBeforeHandle(para);
    }

    //�غ�����ֵ
    public void roundAddValueHandle(ICardHandlePara para)
    {
        _roundAddValueHandle(para);
    }

    //�غ�����ħ��ֵ
    public void roundAddMagicHandle(ICardHandlePara para) {
        _roundAddMagicHandle(para);
    }

    //��������
    public void roundSpecialAttrHandle(ICardHandlePara para)
    {
        _roundSpecialAttrHandle(para);
    }
    //�غϹ���ǰ
    public void roundAttackBeforeHandle(ICardHandlePara para)
    {
        _roundAttackBeforeHandle(para);
    }

    //����
    public void roundAttackHandle(ICardHandlePara para)
    {
        _roundAttackHandle(para);
    }

    //ħ������
    public void roundMagicAttackHandle(ICardHandlePara para) {
        _roundMagicAttackHandle(para);
    }
    //�ۻ���
    public void roundSubDefenseHandle(ICardHandlePara para)
    {
        _roundSubDefenseHandle(para);
    }
    //��Ѫ
    public void roundSubBloodHandle(ICardHandlePara para)
    {
        _roundSubBloodHandle(para);
    }

    //�غϹ�����
    public void roundAttackAfterHandle(ICardHandlePara para)
    {
        _roundAttackAfterHandle(para);
    }
    //�غϽ���
    public void roundEndHandle(ICardHandlePara para)
    {
        _roundEndHandle(para);
    }

    //�����¿���
    protected virtual void _addNewCardAfterHandle(ICardHandlePara para) { }

    //���ƺ�
    protected virtual void _handPokerAfterHandle(ICardHandlePara para) { }
    //Ҫ��
    protected virtual void _dealPokerAfterHandle(ICardHandlePara para){ }

    //�غ�ǰ
    protected virtual void _roundBeginHandle(ICardHandlePara para) { }
    //�غ�����ֵǰ
    protected virtual void _roundAddValueBeforeHandle(ICardHandlePara para) { }
    //�غ�����ֵ
    protected virtual void _roundAddValueHandle(ICardHandlePara para) { }
    //�غ�����ħ��ֵ
    protected virtual void _roundAddMagicHandle(ICardHandlePara para){ }

    //��������
    protected virtual void _roundSpecialAttrHandle(ICardHandlePara para){}

    //�غϹ���ǰ
    protected virtual void _roundAttackBeforeHandle(ICardHandlePara para) { }

    //����
    protected virtual void _roundAttackHandle(ICardHandlePara para) { }

    //ħ������
    protected virtual void _roundMagicAttackHandle(ICardHandlePara para) { }
    //�ۻ���
    protected virtual void _roundSubDefenseHandle(ICardHandlePara para) { }
    //��Ѫ
    protected virtual void _roundSubBloodHandle(ICardHandlePara para) { }

    //�غϹ�����
    protected virtual void _roundAttackAfterHandle(ICardHandlePara para) { }
    //�غϽ���
    protected virtual void _roundEndHandle(ICardHandlePara para) { }

    //����һλС��
    protected float getNumberDigits(float number)
    {
        return (float)Math.Round((number*10+0.5)/10, 1);
    }
}
