//∫ÏÃ“¥Û ¶
public class HeartCardHandle: CardHandleObject
{
    override
    protected void _addValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit != PokerSuit.heart)
        { //∫ÏÃ“
            return;
        }

        float addValue = 1;
        float finalValue = para.getAttackUser().addAttack(addValue);
        IUICardHandlePara uiPara = new UICardHandleParaObject(para, addValue, finalValue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara);
    }
}
