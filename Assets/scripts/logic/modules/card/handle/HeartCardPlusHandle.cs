//红桃大师
public class HeartCardPlusHandle : CardHandleObject
{
    override
    protected void _roundAddValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit == PokerSuit.heart)
        { //红桃
            float addDefense = 0;
            float addValue = getNumberDigits(para.getBaseValue() * 0.5f);
            if (para.getAttackUser().getBlood() + addValue >= para.getAttackUser().getMaxBlood())
            {
                addDefense = para.getAttackUser().getBlood() + addValue - para.getAttackUser().getMaxBlood();
            }
            addValue -= addDefense;

            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "治疗+" + addValue);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            float finalValue = para.getAttackUser().addBlood(addValue);
            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), GameConst.SuitTransformValueType(suit), addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
            
            if (addDefense > 0)
            {
                IUIFlyFontPara uiPara3 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "护甲+" + addDefense);
                GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara3);

                float finalDefense = para.getAttackUser().addDefense(addDefense);
                IUICommonPara uiPara4 = new UICommonParaObject(para.getAttackUser(), ValueType.defense, addDefense, finalDefense);
                GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara4);
            }
        }
    }
}
