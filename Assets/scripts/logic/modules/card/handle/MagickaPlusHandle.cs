//魔能外溢+
public class MagickaPlusHandle : MagickaHandle
{

    override
    protected void _roundSpecialAttrHandle(ICardHandlePara para)
    {
        int number = HandPokerMgr.Instance.getHandPokerPoint(para.getAttackUser(), false);
        if (number == 21) {
            float addValue = 5;
            float finalVlaue = para.getAttackUser().addMagic(addValue);
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "魔法值+" + addValue);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), ValueType.magic, addValue, finalVlaue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);

            CardMgr.Instance.handle(para, CardHandleType.roundAddMagic);
        }
    }

    override
    protected int getNumber() {
        return 2;
    }

    override
    protected int getMaxNumber()
    {
        return 5;
    }
}
