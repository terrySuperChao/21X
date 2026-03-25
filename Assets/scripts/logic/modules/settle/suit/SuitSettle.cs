public class SuitSettle : ISuitSettle
{
    private ISuitSettle _nextSuitSettle;

    public ISuitSettle setNextSuitSettle(ISuitSettle nextSuitSettle) { 
        return this._nextSuitSettle = nextSuitSettle;
    }

    public void settle(ICardHandlePara handlePara, IPoker poker,int baseValue) {
        if (!this._matchSuit(poker.getSuit())) {
            if (this._nextSuitSettle != null) {
                this._nextSuitSettle.settle(handlePara, poker, baseValue);
            }
            return;
        }

        float attrMult = handlePara.getRoundResult().getAttributeMult();
        float addValue = baseValue * attrMult * this._getMult();
        float finalValue = this._getFinalValue(handlePara.getAttackUser(), addValue);
        string text = "+" + (baseValue * this._getMult()) + (attrMult > 1.0f ? " X " + attrMult : "");

        handlePara.setPoker(poker);
        handlePara.setBaseValue(addValue);

        IUIPokerPara pokerPara = new UIPokerPara(handlePara.getAttackUser(), poker, finalValue, text);
        GameMessage.Instance.addMsg(GameConst.ADDPOKERVALUE, pokerPara);

        CardMgr.Instance.handle( handlePara, CardHandleType.roundAddValueBefore);
        CardMgr.Instance.handle( handlePara, CardHandleType.roundAddValue);
    }

    protected virtual float _getFinalValue(IUser attackUser,float value) { return value; }
    protected virtual bool _matchSuit(int suit) { return false; }
    protected virtual float _getMult() { return 1.0f; }
}