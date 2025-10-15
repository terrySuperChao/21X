//狂战士之怒+

public class BerserkerPlusHandle : BerserkerHandle
{
    override
    protected void lossBlood(ICardHandlePara para)
    {
        float addValue = -10;
        float finalValue = para.getAttackUser().addBlood(addValue);

        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "血量" + addValue);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

        IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), ValueType.blood, addValue, finalValue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
    }

    override
    protected int getNumber()
    {
        return 100;
    }
}
