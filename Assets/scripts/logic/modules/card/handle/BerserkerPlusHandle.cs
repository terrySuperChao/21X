//��սʿ֮ŭ+

public class BerserkerPlusHandle : BerserkerHandle
{
    override
    protected void lossBlood(ICardHandlePara para)
    {
        float addValue = -10;
        float finalValue = para.getAttackUser().addBlood(addValue);

        IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), ValueType.blood, addValue, finalValue);
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, uiPara2);
    }

    override
    protected int getNumber()
    {
        return 100;
    }
}
