//ºìÌÒ´óÊ¦
using System;

public class HeartCardPlusHandle : CardHandleObject
{
    override
    protected void _addValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit != PokerSuit.heart)
        { //ºìÌÒ
            return;
        }

        float addDefense = 0;
        float addValue = getNumberDigits(para.getBaseValue() * 0.5f);
        if (para.getAttackUser().getBlood() + addValue >= para.getAttackUser().getMaxBlood())
        {
            addDefense = para.getAttackUser().getBlood() + addValue - para.getAttackUser().getMaxBlood();
        }
        addValue -= addDefense;

        if (addValue > 0) { 
        
            float finalValue = para.getAttackUser().addBlood(addValue);
            IUICardHandlePara uiPara = new UICardHandleParaObject(para, addValue , finalValue, suit);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara);
        }

        if (addDefense > 0) {
            addValue = addDefense;
            float finalValue = para.getAttackUser().addDefense(addValue);
            IUICardHandlePara uiPara = new UICardHandleParaObject(para, addValue, finalValue,PokerSuit.diamond);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara);
        }
    }
}
