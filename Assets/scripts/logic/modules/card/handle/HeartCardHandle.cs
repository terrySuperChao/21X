//∫ÏÃ“¥Û ¶
using System;

public class HeartCardHandle: CardHandleObject
{
    override
    protected void _roundAddValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit == PokerSuit.heart)
        { //∫ÏÃ“
            float addValue = getNumberDigits(para.getBaseValue() * 0.2f);
            float finalValue = para.getAttackUser().addBlood(addValue);
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "÷Œ¡∆+" + addValue);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), GameConst.SuitTransformValueType(suit), addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
        }
    }
}
