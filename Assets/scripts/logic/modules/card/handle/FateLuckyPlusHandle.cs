//ÃüÔËÀ¡Ôù+
public class FateLuckyPlusHandle : CardHandleObject
{
    override
    protected void _dealPokerAfterHandle(ICardHandlePara para)
    {
        if (RandomMgr.Instance.getRangeInt(0, 100) <= 40) {
            PlayPokerMgr.Instance.dealPoker(2);
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "ÊÖÅÆ+2");
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
        }
    }
}
