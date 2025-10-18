//方块大师
public class DiamondCardHandle : CardHandleObject
{
    override
    protected void _roundAddValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit == PokerSuit.diamond)
        { //方块
            
            float addValue = getNumberDigits(para.getBaseValue() * getNumber());
            float finalValue = para.getAttackUser().addDefense(addValue);

            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "护甲+" + addValue);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), GameConst.SuitTransformValueType(suit), addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
        }
    }

    protected virtual float getNumber() {
        return 0.2f;
    }
}
