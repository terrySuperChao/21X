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
        para.setPokerSuit(handPoker.getSuit());

        float outValue = 0;
        float baseValue = handPoker.getBaseValue();
        float addValue = baseValue * this._getMult();
        float finalValue = this._getFinalValue(para, addValue,out outValue);
     
        IUIPokerPara pokerPara = new UIPokerPara(para.getAttackUser(), handPoker.getPokers(), outValue, finalValue, this._getMult());
        GameMessage.Instance.addMsg(GameConst.ADDPOKERVALUE, pokerPara);
        GameCardMgr.Instance.handle(para, TriggerEvent.POST_SUIT_ATTRIBUTE_CONVERSION);

        this._suitSettle(para, handPoker, outValue);
    }

    protected virtual float _getMult() { return 1.0f; }
    protected virtual bool _matchSuit(PokerSuit suit) { return false; }
    protected virtual float _getFinalValue(ITriggerHandlePara para, float value,out float outValue) { outValue = value; return value; }
    protected virtual void _suitSettle(ITriggerHandlePara para, IHandPokerSuit handPoker,float addValue) { }
}