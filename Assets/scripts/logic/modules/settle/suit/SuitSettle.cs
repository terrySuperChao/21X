public class SuitSettle : ISuitSettle
{
    private ISuitSettle _nextSuitSettle;

    public ISuitSettle setNextSuitSettle(ISuitSettle nextSuitSettle) {
        return this._nextSuitSettle = nextSuitSettle;
    }

    public void settle(ITriggerHandlePara para, IHandPokerSuit handPoker) {
        if (!this._matchSuit(handPoker.getSuit())) {
            if (this._nextSuitSettle != null) {
                this._nextSuitSettle.settle(para, handPoker);
            }
            return;
        }

        float baseValue = handPoker.getBaseValue();
        float attrMult = para.getRoundResult(para.getAttackUser()).getAttributeMult();
        float addValue = baseValue * attrMult * this._getMult();
        float finalValue = this._getFinalValue(para.getAttackUser(), addValue);
        float showValue = baseValue * this._getMult();
      
        para.setBaseValue(addValue);
        para.setPokerSuit(handPoker.getSuit());

        IUIPokerPara pokerPara = new UIPokerPara(para.getAttackUser(), handPoker.getPokers(), showValue, finalValue, this._getMult());
        GameMessage.Instance.addMsg(GameConst.ADDPOKERVALUE, pokerPara);

        this._settle(para, handPoker);

        CardMgr.Instance.handle(para, TriggerEvent.transformAttribute);
    }

    protected virtual float _getFinalValue(IUser attackUser, float value) { return value; }
    protected virtual bool _matchSuit(PokerSuit suit) { return false; }
    protected virtual float _getMult() { return 1.0f; }
    protected virtual void _settle(ITriggerHandlePara para, IHandPokerSuit handPoker) { }
}