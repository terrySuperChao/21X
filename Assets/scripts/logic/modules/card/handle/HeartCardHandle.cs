//∫ÏÃ“¥Û ¶
using System;

public class HeartCardHandle: CardHandleObject
{
    override
    protected void _addValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit != PokerSuit.heart)
        { //∫ÏÃ“
            return;
        }

        float addValue = getNumberDigits(para.getBaseValue() * 0.2f);
        float finalValue = para.getAttackUser().addBlood(addValue);
        IUICardHandlePara uiPara = new UICardHandleParaObject(para, addValue, finalValue, suit);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara);
    }
}
