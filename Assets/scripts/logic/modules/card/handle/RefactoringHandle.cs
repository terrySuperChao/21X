//牌序重构
using UnityEngine;
public class RefactoringHandle : CardHandleObject
{
    override
    protected void _addNewCardAfterHandle(ICardHandlePara para)
    {
        handle(para);
    }

    override
    protected void _handPokerAfterHandle(ICardHandlePara para)
    {
        handle(para);
    }

    protected void handle(ICardHandlePara para) {
        int number = getNumber();
        GameMessage.Instance.addMsg(GameConst.REFACTORING, para.getUser(), number);

        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getUser(), para.getCard(), "重开+"+ number);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
    }

    protected virtual int getNumber() {
        return 1;
    }
}
