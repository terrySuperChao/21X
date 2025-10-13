//∫⁄Ã“¥Û ¶
public class SpadeCardPlusHandle : SpadeCardHandle
{
    private bool isPenetrate = false;
    override
    protected void _roundAttackBeforeHandle(ICardHandlePara para)
    {
        if (isPenetrate = RandomMgr.Instance.getRangeInt(0, 2) == 0)
        {
            para.getRoundResult().setPenetrateValue(1);
        }
    }

    override
    protected void _roundAttackHandle(ICardHandlePara para){
        if(isPenetrate){
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "¥©Õ∏");
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
        }
    }

    override
    public int getNumber()
    {
        return 1;
    }
}
