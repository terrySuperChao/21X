//±¬ÅÆÖ®¶Ü+
public class ExplosiveShieldPlusHandle : CardHandleObject
{
    override
    protected void _roundAttackBeforeHandle(ICardHandlePara para)
    {
        int number = HandPokerMgr.Instance.getHandPokerPoint(para.getUser(), false);
        if (number > 21)
        {
            bool isDouble = RandomMgr.Instance.getRangeInt(0, 2) == 0;
            string text = isDouble ? "»¤¼×+10x2" : "»¤¼×+10";
            float addValue = isDouble ? 20 : 10;
            float finalVlaue = para.getUser().addDefense(addValue);
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getUser(), para.getCard(), text);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getUser(), ValueType.defense, addValue, finalVlaue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
        }
    }
}
