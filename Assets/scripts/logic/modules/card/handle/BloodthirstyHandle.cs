//ÊÈÑª¿ñ»¶
using System;
using UnityEngine;

public class BloodthirstyHandle : CardHandleObject
{
    private bool isMult = false;
    override
    protected void _roundBeginHandle(ICardHandlePara para)
    {
        isMult = para.getAttackUser().getBlood() / para.getAttackUser().getMaxBlood() <= getRatio();
        if (isMult)
        {
            para.getRoundResult().setAttributeMult(getNumber());
        }
    }

    override
    protected void _roundAddValueHandle(ICardHandlePara para)
    {
        if (isMult) {
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "ÊôÐÔX" + getNumber());
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
