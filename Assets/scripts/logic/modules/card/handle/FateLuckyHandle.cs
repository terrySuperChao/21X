//ÃüÔËÀ¡Ôù
using UnityEngine;
public class FateLuckyHandle : CardHandleObject
{
    override
    protected void _dealPokerAfterHandle(ICardHandlePara para)
    {
        if (RandomMgr.Instance.getRangeInt(0, 100) <= 40) {
            IUser user = PlayPokerMgr.Instance.getNoneStateUser(!para.getUser().isNpc());
            if (user != null) {
                int number = getNumber();
                IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getUser(), para.getCard(), "ÊÖÅÆ+"+ number);
                GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
                PlayPokerMgr.Instance.specialUserDealPoker(user, number);
            }   
        }
    }

    protected virtual int getNumber()
    {
        return 1;
    }
}
