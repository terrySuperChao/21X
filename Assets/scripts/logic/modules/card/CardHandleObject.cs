using System;

public abstract class CardHandleObject : ICardHandle
{

    //添加新卡牌
    public void addNewCardAfterHandle(ICardHandlePara para) {
        _addNewCardAfterHandle(para);
    }

    //手牌后
    public void handPokerAfterHandle(ICardHandlePara para) {
        _handPokerAfterHandle(para);
    }
    //要牌后
    public void dealPokerAfterHandle(ICardHandlePara para) {
        _dealPokerAfterHandle(para);
    }

    //回合前
    public void roundBeginHandle(ICardHandlePara para)
    {
        _roundBeginHandle(para);
    }

    //回合添加值前
    public void roundAddValueBeforeHandle(ICardHandlePara para) {
        _roundAddValueBeforeHandle(para);
    }

    //回合添加值
    public void roundAddValueHandle(ICardHandlePara para)
    {
        _roundAddValueHandle(para);
    }

    //回合添加魔法值
    public void roundAddMagicHandle(ICardHandlePara para) {
        _roundAddMagicHandle(para);
    }

    //爆牌
    public void roundBustHandle(ICardHandlePara para)
    {
        _roundBustHandle(para);
    }
    //回合攻击前
    public void roundAttackBeforeHandle(ICardHandlePara para)
    {
        _roundAttackBeforeHandle(para);
    }

    //攻击
    public void roundAttackHandle(ICardHandlePara para)
    {
        _roundAttackHandle(para);
    }

    //魔法攻击
    public void roundMagicAttackHandle(ICardHandlePara para) {
        _roundMagicAttackHandle(para);
    }
    //扣护甲
    public void roundSubDefenseHandle(ICardHandlePara para)
    {
        _roundSubDefenseHandle(para);
    }
    //扣血
    public void roundSubBloodHandle(ICardHandlePara para)
    {
        _roundSubBloodHandle(para);
    }

    //回合攻击后
    public void roundAttackAfterHandle(ICardHandlePara para)
    {
        _roundAttackAfterHandle(para);
    }
    //回合结束
    public void roundEndHandle(ICardHandlePara para)
    {
        _roundEndHandle(para);
    }

    //添加新卡牌
    protected virtual void _addNewCardAfterHandle(ICardHandlePara para) { }

    //手牌后
    protected virtual void _handPokerAfterHandle(ICardHandlePara para) { }
    //要牌
    protected virtual void _dealPokerAfterHandle(ICardHandlePara para){ }

    //回合前
    protected virtual void _roundBeginHandle(ICardHandlePara para) { }
    //回合添加值前
    protected virtual void _roundAddValueBeforeHandle(ICardHandlePara para) { }
    //回合添加值
    protected virtual void _roundAddValueHandle(ICardHandlePara para) { }
    //回合添加魔法值
    protected virtual void _roundAddMagicHandle(ICardHandlePara para){ }

    //爆牌
    protected virtual void _roundBustHandle(ICardHandlePara para){}

    //回合攻击前
    protected virtual void _roundAttackBeforeHandle(ICardHandlePara para) { }

    //攻击
    protected virtual void _roundAttackHandle(ICardHandlePara para) { }

    //魔法攻击
    protected virtual void _roundMagicAttackHandle(ICardHandlePara para) { }
    //扣护甲
    protected virtual void _roundSubDefenseHandle(ICardHandlePara para) { }
    //扣血
    protected virtual void _roundSubBloodHandle(ICardHandlePara para) { }

    //回合攻击后
    protected virtual void _roundAttackAfterHandle(ICardHandlePara para) { }
    //回合结束
    protected virtual void _roundEndHandle(ICardHandlePara para) { }

    //保留一位小数
    protected float getNumberDigits(float number)
    {
        return (float)Math.Round(number, 1);
    }
}
