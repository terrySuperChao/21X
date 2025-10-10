//ºÚÌÒ´óÊ¦
using System;
using UnityEngine;
public class SpadeCardPlusHandle : CardHandleObject
{
    private bool isPenetrate = false;
    override
    protected void _roundAttackBeginHandle(ICardHandlePara para)
    {
        if (isPenetrate = RandomMgr.Instance.getRangeInt(0, 2) == 0)
        {
            para.getRoundResult().setPenetrateValue(1);
        }
    }

    override
    protected void _roundAddValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit == PokerSuit.spade)
        {   //ºÚÌÒ
            float addValue = 2;
            float finalValue = para.getAttackUser().addAttack(addValue);
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "¹¥»÷Á¦+" + addValue);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), GameConst.SuitTransformValueType(suit), addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
        }
    }

    override
    protected void _roundAttackHandle(ICardHandlePara para){
        if(isPenetrate){
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "´©Í¸");
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
        }
    }
}
