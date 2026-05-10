public class SuitSettle : ISuitSettle
{
    private ISuitSettle _nextSuitSettle;

    public ISuitSettle setNextSuitSettle(ISuitSettle nextSuitSettle) { 
        return this._nextSuitSettle = nextSuitSettle;
    }

    public void settle(ITriggerHandlePara para, IPoker poker,int baseValue) {
        if (!this._matchSuit(poker.getSuit())) {
            if (this._nextSuitSettle != null) {
                this._nextSuitSettle.settle(para, poker, baseValue);
            }
            return;
        }

        float attrMult = para.getRoundResult(para.getAttackUser()).getAttributeMult();
        float addValue = baseValue * attrMult * this._getMult();
        float finalValue = this._getFinalValue(para.getAttackUser(), addValue);
        string text = "+" + (baseValue * this._getMult()) + (attrMult > 1.0f ? " X " + attrMult : "");

        para.setPoker(poker);
        para.setBaseValue(addValue);

        IUIPokerPara pokerPara = new UIPokerPara(para.getAttackUser(), poker, finalValue, text);
        GameMessage.Instance.addMsg(GameConst.ADDPOKERVALUE, pokerPara);
        CardMgr.Instance.handle(para, TriggerEvent.transformAttribute);
    }

    protected virtual float _getFinalValue(IUser attackUser,float value) { return value; }
    protected virtual bool _matchSuit(int suit) { return false; }
    protected virtual float _getMult() { return 1.0f; }
}