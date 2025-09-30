//方块大师
using System;

public class DiamondCardPlusHandle : CardHandleObject
{
    override
    protected void _addValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit != PokerSuit.diamond)
        { 
            return;
        }
        float addValue = getNumberDigits(para.getBaseValue() * 0.5f);
        float finalValue = para.getAttackUser().addDefense(addValue);
        IUICardHandlePara uiPara = new UICardHandleParaObject(para, addValue , finalValue, suit);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara);
    }

    override
   protected void _addRoundValueHanle(ICardHandlePara para)
    {
        float attack = para.getAttackUser().getAttack() * 0.5f;
        if (attack <= 0) return;
        para.getAttackUser().addBlood(-attack);
        para.getRoundResult().setReflectValue(attack);
        IUICardHandlePara uiPara = new UICardHandleParaObject(para, 0, 0, PokerSuit.spade);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara, "反弹伤害"+attack,1.0f);
    }
}
