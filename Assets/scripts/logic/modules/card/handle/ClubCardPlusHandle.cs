//梅花大师
public class ClubCardPlusHandle : ClubCardHandle
{
   
    override
    protected void _roundAttackBeforeHandle(ICardHandlePara para)
    {
          para.getRoundResult().setSaveMagicValue(para.getAttackUser().getMaxMagic() * 0.3f);
    }


    override
    protected void _roundMagicAttackHandle(ICardHandlePara para)
    {
        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "保留30%魔法");
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
    }

    override
    protected int getNumber()
    {
        return 2;
    }
    
}
