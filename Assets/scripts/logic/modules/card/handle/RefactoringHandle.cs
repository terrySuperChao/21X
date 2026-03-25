//牌序重构
using UnityEngine;
public class RefactoringHandle : CardHandleObject
{
    private bool isFirst = true;
    override
    protected void _addNewCardAfterHandle(ICardHandlePara para)
    {
        if (isFirst) {
            isFirst = false;
            handle(para);
            Debug.Log("RefactoringHandle");
        }
    }

    override
    protected void _handPokerAfterHandle(ICardHandlePara para)
    {
        handle(para);
    }

    protected void handle(ICardHandlePara para) {
        int number = getNumber();
        GameMessage.Instance.addMsg(GameConst.REFACTORING, new RefactoringPara(para.getUser(), number));

        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getUser(), para.getCard(), "重开+"+ number);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
    }

    protected virtual int getNumber() {
        return 1;
    }
}
