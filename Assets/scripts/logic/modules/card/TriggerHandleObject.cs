using System;

public abstract class TriggerHandleObject : ITriggerHandle
{
    public TriggerEvent getTrigger() {
        return this._getTrigger();
    }

    public void addNewCardAfterHandle(ITriggerHandlePara para)
    {
        this._addNewCardAfterHandle(para);
    }

    public void handPokerAfterHandle(ITriggerHandlePara para)
    {
        this._handPokerAfterHandle(para);
    }

    public void dealPokerAfterHandle(ITriggerHandlePara para)
    {
        this._dealPokerAfterHandle(para);
    }

    public void roundBeginHandle(ITriggerHandlePara para)
    {
        this._roundBeginHandle(para);
    }
  
    public void roundAddValueBeforeHandle(ITriggerHandlePara para)
    {
        this._roundAddValueBeforeHandle(para);
    }

    public void roundAddValueHandle(ITriggerHandlePara para)
    {
        this._roundAddValueHandle(para);
    }

    public void roundAddMagicHandle(ITriggerHandlePara para)
    {
        this._roundAddMagicHandle(para);
    }

    public void roundSpecialAttrHandle(ITriggerHandlePara para)
    {
        this._roundSpecialAttrHandle(para);
    }
    
    public void roundAttackBeforeHandle(ITriggerHandlePara para)
    {
        this._roundAttackBeforeHandle(para);
    }

    public void roundAttackHandle(ITriggerHandlePara para)
    {
        this._roundAttackHandle(para);
    }

    public void roundMagicAttackHandle(ITriggerHandlePara para)
    {
        this._roundMagicAttackHandle(para);
    }

    public void roundSubDefenseHandle(ITriggerHandlePara para)
    {
        this._roundSubDefenseHandle(para);
    }

    public void roundSubBloodHandle(ITriggerHandlePara para)
    {
        this._roundSubBloodHandle(para);
    }

    public void roundAttackAfterHandle(ITriggerHandlePara para)
    {
        this._roundAttackAfterHandle(para);
    }
    
    public void roundEndHandle(ITriggerHandlePara para)
    {
        this._roundEndHandle(para);
    }
    protected float getNumberDigits(float number)
    {
        return (float)Math.Round((number * 10 + 0.5) / 10, 1);
    }

    protected virtual TriggerEvent _getTrigger() { return 0; }
    protected virtual void _addNewCardAfterHandle(ITriggerHandlePara para) { }
    protected virtual void _handPokerAfterHandle(ITriggerHandlePara para) { }
    protected virtual void _dealPokerAfterHandle(ITriggerHandlePara para) { }
    protected virtual void _roundBeginHandle(ITriggerHandlePara para) { }
    protected virtual void _roundAddValueBeforeHandle(ITriggerHandlePara para) { }
    protected virtual void _roundAddValueHandle(ITriggerHandlePara para) { }
    protected virtual void _roundAddMagicHandle(ITriggerHandlePara para) { }
    protected virtual void _roundSpecialAttrHandle(ITriggerHandlePara para) { }
    protected virtual void _roundAttackBeforeHandle(ITriggerHandlePara para) { }
    protected virtual void _roundAttackHandle(ITriggerHandlePara para) { }
    protected virtual void _roundMagicAttackHandle(ITriggerHandlePara para) { }
    protected virtual void _roundSubDefenseHandle(ITriggerHandlePara para) { }
    protected virtual void _roundSubBloodHandle(ITriggerHandlePara para) { }
    protected virtual void _roundAttackAfterHandle(ITriggerHandlePara para) { }
    protected virtual void _roundEndHandle(ITriggerHandlePara para) { }

    
}
