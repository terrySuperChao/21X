//狂战士之怒+
using System;

public class BerserkerPlusHandle : CardHandleObject
{
    private float _saveAttackValue = 0.0f;
    override
    protected void _roundAttackBeginHandle(ICardHandlePara para)
    {
        _saveAttackValue = getNumberDigits(para.getAttackUser().getAttack() * 1.0f);
        if (_saveAttackValue >= 0.1f) {
            para.getRoundResult().setSaveAttackValue(_saveAttackValue);
        }
    }

    override
    protected void _roundAttackHandle(ICardHandlePara para)
    {
        if (_saveAttackValue >= 0.1f) {
            float addValue = -10;
            float finalValue = para.getAttackUser().addBlood(addValue);

            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "攻击力保留100%");
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
                       
            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), ValueType.blood, addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.COMMONATTACK, uiPara2);
        }
    }
}
