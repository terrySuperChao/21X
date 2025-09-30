//∫⁄Ã“¥Û ¶
using System;
using UnityEngine;
public class SpadeCardPlusHandle : CardHandleObject
{
    override
    protected void _addValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        //∫⁄Ã“
        if (suit != PokerSuit.spade) return;
        
        float addValue = 2;
        float finalValue = para.getAttackUser().addAttack(addValue);
        IUICardHandlePara uiPara = new UICardHandleParaObject(para, addValue, finalValue, suit);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara);
    }

    override
    protected void _addRoundValueHanle(ICardHandlePara para)
    {
        if (RandomMgr.Instance.getRangeInt(0,2) == 0) return;

        para.getRoundResult().setPenetrateValue(1);
        IUICardHandlePara uiPara = new UICardHandleParaObject(para, 0, 0, PokerSuit.spade);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara, "¥©Õ∏",0.0f);
        
    }
}
