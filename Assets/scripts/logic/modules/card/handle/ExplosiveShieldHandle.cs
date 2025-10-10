//±¬ÅÆÖ®¶Ü
using System;
using UnityEngine;

public class ExplosiveShieldHandle : CardHandleObject
{

    override
    protected void _roundAttackBeginHandle(ICardHandlePara para)
    {
        int number = HandPokerMgr.Instance.getHandPokerPoint(para.getAttackUser(), false);
        if (number > 21)
        {
            float addValue = 5;
            float finalVlaue = para.getAttackUser().addDefense(addValue);
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "»¤¼×+" + addValue);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), ValueType.defense, addValue, finalVlaue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
        }
    }
}
