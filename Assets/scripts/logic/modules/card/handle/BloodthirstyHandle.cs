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
        int point = HandPokerMgr.Instance.getHandPokerPoint(para.getAttackUser(), false);
        if(point == 21)
        {
            if (para.getAttackUser().getMaxBlood() * getRatio() >= para.getAttackUser().getBlood())
            {
                para.getRoundResult().setAttributeMult(getNumber());
                isMult = true;
            }
        }
        
    }

    override
    protected void _roundAddValueHandle(ICardHandlePara para)
    {
        if (isMult) {
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), " Ù–‘X" + getNumber());
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
        }
    }

    protected virtual int getNumber()
    {
        return 2;
    }

    protected virtual float getRatio()
    {
        return 0.25f;
    }

}
