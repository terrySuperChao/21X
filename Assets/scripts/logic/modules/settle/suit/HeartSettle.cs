public class HeartSettle : SuitSettle
{
    protected override float _getFinalValue(IUser attackUser, float value) {
        return attackUser.addBlood(value);
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.heart; 
    }

    protected override float _getMult() { return 0.5f; }

    protected override void _settle(ITriggerHandlePara para, IHandPokerSuit handPoker)
    {
        float addValue = handPoker.getBaseValue() * para.getAttackUser().getExtraInfo().getHealToMP();
        if (addValue > 0)
        {
            para.getAttackUser().addMagic(addValue);
            IUICommonPara attackPara = new UICommonParaObject(para.getAttackUser(), ValueType.magic, addValue, para.getAttackUser().getMagic());
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
        }
    }
}