//方块大师+
using System;

public class DiamondCardPlusHandle : CardHandleObject
{
    override
    protected void _roundAddValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit == PokerSuit.diamond)
        { 
            float addValue = getNumberDigits(para.getBaseValue() * 0.5f);
            float finalValue = para.getAttackUser().addDefense(addValue);

            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "护甲+" + addValue);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), GameConst.SuitTransformValueType(suit), addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
        }
    }

   override
   protected void _roundSubDefenseHandle(ICardHandlePara para)
    {
        float value = getNumberDigits(para.getAttackUser().getAttack() * 0.5f);
        if (value >= 0.1f) {
            float addValue = -value;
            float finalValue = para.getAttackUser().addBlood(addValue);
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getDefenseUser(), para.getCard(), "反弹伤害" + value);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), ValueType.blood, addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
        }
    }
}
