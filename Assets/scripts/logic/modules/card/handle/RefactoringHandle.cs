//牌序重构
public class RefactoringHandle : CardHandleObject
{
    override
    protected void _handPokerAfterHandle(ICardHandlePara para)
    {
        GameMessage.Instance.addMsg(GameConst.REFACTORING, para.getUser(),1);

        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getUser(), para.getCard(), "重开+1");
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
    }
}
