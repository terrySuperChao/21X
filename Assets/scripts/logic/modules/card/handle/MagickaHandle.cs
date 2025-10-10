//Ä§ÄÜÍâÒç
using System;

public class MagickaHandle : CardHandleObject
{
    override
    protected void _roundAddMagicHandle(ICardHandlePara para)
    {
        float value = RandomMgr.Instance.getRangeInt(1, 3);
        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "Ä§·¨ÉËº¦+" + value);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

        float addValue = -value;
        float finalVlaue = para.getDefenseUser().addBlood(addValue);
        IUICommonPara uiPara2 = new UICommonParaObject(para.getDefenseUser(), ValueType.blood, addValue, finalVlaue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
    }
}
