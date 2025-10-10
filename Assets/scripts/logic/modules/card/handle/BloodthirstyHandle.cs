// »—™øÒª∂
using System;
using UnityEngine;

public class BloodthirstyHandle : CardHandleObject
{
    private bool isMult = false;
    override
    protected void _roundBeginHandle(ICardHandlePara para)
    {
        isMult = false;
        int number = HandPokerMgr.Instance.getHandPokerPoint(para.getAttackUser(), false);
        if(number == 21)
        {
            if (para.getAttackUser().getMaxBlood() * 0.25f > para.getAttackUser().getBlood())
            {
                para.getRoundResult().setAttributeMult(2);
                isMult = true;
            }
        }
        
    }

    override
    protected void _roundAddValueHandle(ICardHandlePara para)
    {
        if (isMult) {
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), " Ù–‘X2");
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
        }
    }

}
