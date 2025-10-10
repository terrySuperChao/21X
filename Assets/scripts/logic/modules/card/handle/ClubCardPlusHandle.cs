//梅花大师
using System;

public class ClubCardPlusHandle : CardHandleObject
{
    override
    protected void _roundAddValueHandle(ICardHandlePara para) {
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit == PokerSuit.club)
        {
            float addValue = 2;
            float finalValue = para.getAttackUser().addMagic(addValue);

            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "魔法+" + addValue);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), GameConst.SuitTransformValueType(suit), addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);

            CardMgr.Instance.handle(para, CardHandleType.roundAddMagic);
        }
    }

    override
    protected void _roundAttackBeginHandle(ICardHandlePara para)
    {
        para.getRoundResult().setSaveMagicValue(para.getAttackUser().getMaxMagic() * 0.3f);
    }


    override
    protected void _roundMagicAttackHandle(ICardHandlePara para)
    {
        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "保留30%魔法");
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
    }
    
}
