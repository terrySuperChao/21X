//魔能外溢
public class MagickaHandle : CardHandleObject
{
    override
    protected void _roundAddValueBeforeHandle(ICardHandlePara para)
    { 
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit == PokerSuit.club)
        { //梅花
            _roundAddMagicHandle(para);
        }
    }

    override
    protected void _roundAddMagicHandle(ICardHandlePara para)
    {
        float value = RandomMgr.Instance.getRangeInt(1, 3);
        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "魔法伤害+" + value);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

        float addValue = -value;
        float finalVlaue = para.getDefenseUser().addBlood(addValue);
        IUICommonPara uiPara2 = new UICommonParaObject(para.getDefenseUser(), ValueType.blood, addValue, finalVlaue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
    }
}
