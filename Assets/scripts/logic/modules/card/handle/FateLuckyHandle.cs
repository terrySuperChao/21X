//ÃüÔËÀ¡Ôù
using System.Collections.Generic;
using UnityEngine;
public class FateLuckyHandle : CardHandleObject
{
    override
    protected void _dealPokerAfterHandle(ICardHandlePara para)
    {
        if (RandomMgr.Instance.getRangeInt(0, 100) <= 40) {
            List<IUser> users = PlayPokerMgr.Instance.getPlayers();
            for (int i = 0; i < users.Count; i++) {
                if (users[i].isNpc() != para.getUser().isNpc()) {
                    int number = getNumber();
                    IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getUser(), para.getCard(), "ÊÖÅÆ+" + number);
                    GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
                    PlayPokerMgr.Instance.specialUserDealPoker(users[i], number);
                    break;
                }
            }
        }
    }

    protected virtual int getNumber()
    {
        return 1;
    }
}
