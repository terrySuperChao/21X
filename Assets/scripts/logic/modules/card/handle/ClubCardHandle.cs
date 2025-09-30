//√∑ª®¥Û ¶
using System;

public class ClubCardHandle: CardHandleObject
{
    override
    protected void _addValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit != PokerSuit.club)
        {
            return;
        }

        float addValue = 1;
        float finalValue = para.getAttackUser().addMagic(addValue);
        IUICardHandlePara uiPara = new UICardHandleParaObject(para, addValue, finalValue, suit);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara);
    }
}
