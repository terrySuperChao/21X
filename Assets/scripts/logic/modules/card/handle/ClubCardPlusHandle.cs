//√∑ª®¥Û ¶
using System;

public class ClubCardPlusHandle : CardHandleObject
{
    override
    protected void _addValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit != PokerSuit.club)
        {
            return;
        }

        float addValue = 2;
        float finalValue = para.getAttackUser().addMagic(addValue);
        IUICardHandlePara uiPara = new UICardHandleParaObject(para, addValue, finalValue, suit);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara);
    }

    override
    protected void _addRoundValueHanle(ICardHandlePara para)
    {
        para.getRoundResult().setSaveMagicValue(0.3f);
    }
}
