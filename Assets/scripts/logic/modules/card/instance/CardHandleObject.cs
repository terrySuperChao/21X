using System;

public abstract class CardHandleObject : ICardHandle
{
    public void addNewCardAfterHandle(ICardHandlePara para) {
        _addNewCardAfterHandle(para);
    }
    public void handPokerAfterHandle(ICardHandlePara para) {
        _handPokerAfterHandle(para);
    }
    
    public void dealPokerAfterHandle(ICardHandlePara para) {
        _dealPokerAfterHandle(para);
    }

    public void roundBeginHandle(ICardHandlePara para)
    {
        _roundBeginHandle(para);
    }

    public void roundAddValueBeforeHandle(ICardHandlePara para) {
        _roundAddValueBeforeHandle(para);
    }

    public void roundAddValueHandle(ICardHandlePara para)
    {
        _roundAddValueHandle(para);
    }

    public void roundAddMagicHandle(ICardHandlePara para) {
        _roundAddMagicHandle(para);
    }

    public void roundSpecialAttrHandle(ICardHandlePara para)
    {
        _roundSpecialAttrHandle(para);
    }

    public void roundAttackBeforeHandle(ICardHandlePara para)
    {
        _roundAttackBeforeHandle(para);
    }

    public void roundAttackHandle(ICardHandlePara para)
    {
        _roundAttackHandle(para);
    }

    public void roundMagicAttackHandle(ICardHandlePara para) {
        _roundMagicAttackHandle(para);
    }
    
    public void roundSubDefenseHandle(ICardHandlePara para)
    {
        _roundSubDefenseHandle(para);
    }
    
    public void roundSubBloodHandle(ICardHandlePara para)
    {
        _roundSubBloodHandle(para);
    }

    public void roundAttackAfterHandle(ICardHandlePara para)
    {
        _roundAttackAfterHandle(para);
    }

    public void roundEndHandle(ICardHandlePara para)
    {
        _roundEndHandle(para);
    }

    protected virtual void _addNewCardAfterHandle(ICardHandlePara para) { }

    protected virtual void _handPokerAfterHandle(ICardHandlePara para) { }
   
    protected virtual void _dealPokerAfterHandle(ICardHandlePara para){ }

    protected virtual void _roundBeginHandle(ICardHandlePara para) { }
    
    protected virtual void _roundAddValueBeforeHandle(ICardHandlePara para) { }
    
    protected virtual void _roundAddValueHandle(ICardHandlePara para) { }
    
    protected virtual void _roundAddMagicHandle(ICardHandlePara para){ }

    protected virtual void _roundSpecialAttrHandle(ICardHandlePara para){}

    protected virtual void _roundAttackBeforeHandle(ICardHandlePara para) { }

    protected virtual void _roundAttackHandle(ICardHandlePara para) { }

    protected virtual void _roundMagicAttackHandle(ICardHandlePara para) { }

    protected virtual void _roundSubDefenseHandle(ICardHandlePara para) { }

    protected virtual void _roundSubBloodHandle(ICardHandlePara para) { }

    protected virtual void _roundAttackAfterHandle(ICardHandlePara para) { }
   
    protected virtual void _roundEndHandle(ICardHandlePara para) { }
}
