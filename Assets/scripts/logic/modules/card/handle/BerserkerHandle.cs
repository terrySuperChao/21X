//狂战士之怒
using System;
using UnityEngine;

public class BerserkerHandle : CardHandleObject
{
    private float _saveAttackValue = 0.0f;
    override
    protected void _roundAttackBeginHandle(ICardHandlePara para)
    {
        _saveAttackValue = getNumberDigits(para.getAttackUser().getAttack() * 0.2f);
        if (_saveAttackValue >= 0.1f) {
            para.getRoundResult().setSaveAttackValue(_saveAttackValue);
        }
    }

    override
    protected void _roundAttackHandle(ICardHandlePara para) {
        if (_saveAttackValue >= 0.1f) {
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getDefenseUser(), para.getCard(), "攻击力保留20%");
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
        }
    }
}
