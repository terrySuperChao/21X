//±¬ÅÆÖ®¶Ü
public class ExplosiveShieldHandle : CardHandleObject
{
    override
    protected void _roundSpecialAttrHandle(ICardHandlePara para)
    {
        int number = HandPokerMgr.Instance.getHandPokerPoint(para.getUser(), false);
        if (number <= 21)
        {
            return;
        }

        float addValue = getNumber();
        float finalVlaue = para.getUser().addDefense(addValue);
        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getUser(), para.getCard(), "»¤¼×+" + addValue);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

        IUICommonPara uiPara2 = new UICommonParaObject(para.getUser(), ValueType.defense, addValue, finalVlaue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
        
    }

    protected virtual int getNumber()
    {
        return 5;
    }
}
