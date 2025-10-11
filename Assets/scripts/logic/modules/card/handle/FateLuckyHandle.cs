//ÃüÔËÀ¡Ôù
using UnityEngine;
public class FateLuckyHandle : CardHandleObject
{
    override
    protected void _dealPokerAfterHandle(ICardHandlePara para)
    {
        if (RandomMgr.Instance.getRangeInt(0, 100) <= 40) {
            IUser user = PlayPokerMgr.Instance.getNoneStateUser(!para.getAttackUser().isNpc());
            if (user != null) {
                IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "ÊÖÅÆ+1");
                GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
                PlayPokerMgr.Instance.specialUserDealPoker(user,1);
            }   
        }
    }
}
