public class HeartSettle : SuitSettle
{
    protected override float _getFinalValue(ITriggerHandlePara para, float value) {
        return GameBloodMgr.Instance.handle(para.getAttackUser(), value,false);
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.heart; 
    }

    protected override float _getMult() { return 0.5f; }

    protected override void _suitSettle(ITriggerHandlePara para, IHandPokerSuit handPoker,float addValue)
    {
        addValue *= CardMgr.Instance.getBaseEffectValue(para.getAttackUser(), BaseEffectType.healToMP);
        if (addValue > 0)
        {
            para.getAttackUser().addMagic(addValue);
            IUICommonPara attackPara = new UICommonParaObject(para.getAttackUser(), ValueType.magic, addValue, para.getAttackUser().getMagic());
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
        }
    }
}