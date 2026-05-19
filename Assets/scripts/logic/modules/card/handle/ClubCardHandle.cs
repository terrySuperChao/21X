//÷����ʦ
public class ClubCardHandle: CardHandleObject
{
    override
    protected void _roundAddValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit == PokerSuit.club)
        { //÷��

            float addValue = getNumber();
            float finalValue = para.getAttackUser().addMagic(addValue);

            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "ħ��+" + addValue);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), GameUtils.SuitTransformValueType(suit), addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);

            CardMgr.Instance.handle(para, CardHandleType.roundAddMagic);
        }
    }

    protected virtual int getNumber() {
        return 1;
    }
}
