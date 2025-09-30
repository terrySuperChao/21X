//·½¿é´óÊ¦
using System;

public class DiamondCardHandle : CardHandleObject
{
    override
    protected void _addValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit != PokerSuit.diamond)
        { 
            return;
        }

        float addValue = getNumberDigits(para.getBaseValue() * 0.2f);
        float finalValue = para.getAttackUser().addDefense(addValue);
        IUICardHandlePara uiPara = new UICardHandleParaObject(para, addValue, finalValue, suit);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara);
    }
}
