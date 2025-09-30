//∫⁄Ã“¥Û ¶
using UnityEngine;
public class SpadeCardHandle:CardHandleObject
{
    override
    protected void _addValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit != PokerSuit.spade)
        { //∫⁄Ã“
            return;
        }
        
        float addValue = 1;
        float finalValue = para.getAttackUser().addAttack(addValue);
        IUICardHandlePara uiPara = new UICardHandleParaObject(para, addValue, finalValue, suit);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara);
    }
}
