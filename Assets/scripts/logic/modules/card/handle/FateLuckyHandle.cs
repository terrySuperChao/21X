//ÃüÔËÀ¡Ôù
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class FateLuckyHandle : CardHandleObject
{
    override
    protected void _dealPokerHandle(ICardHandlePara para)
    {
        if (RandomMgr.Instance.getRangeInt(0, 100) <= 40) {
            PlayPokerMgr.Instance.dealPoker();
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "ÊÖÅÆ+1");
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
        }
    }
}
