//魔能外溢
using System;
using UnityEngine;

public class MagickaPlusHandle : CardHandleObject
{

    override
    protected void _roundAddMagicHandle(ICardHandlePara para)
    {
        float addValue = RandomMgr.Instance.getRangeInt(2, 5);
        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "魔法伤害+" + addValue);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

        float finalVlaue = para.getDefenseUser().addBlood(-addValue);
        IUICommonPara uiPara2 = new UICommonParaObject(para.getDefenseUser(), ValueType.blood, -addValue, finalVlaue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
    }

    override
    protected void _roundEndHandle(ICardHandlePara para)
    {
        int number = HandPokerMgr.Instance.getHandPokerPoint(para.getAttackUser(), false);
        if (number == 21) {
            float addValue = 5;
            float finalVlaue = para.getAttackUser().addMagic(addValue);
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "魔法值+" + addValue);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), ValueType.magic, addValue, finalVlaue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
        }
    }
}
