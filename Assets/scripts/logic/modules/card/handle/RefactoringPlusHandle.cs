//牌序重构+
public class RefactoringPlusHandle : CardHandleObject
{
    override
    protected void _handPokerAfterHandle(ICardHandlePara para)
    {
        GameMessage.Instance.addMsg(GameConst.REFACTORING, para.getUser(),2);

        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getUser(), para.getCard(), "重开+2");
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
    }
}
