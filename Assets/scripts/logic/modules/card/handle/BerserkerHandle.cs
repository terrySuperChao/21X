//狂战士之怒

public class BerserkerHandle : CardHandleObject
{
    private float _saveAttackValue = 0.0f;
    override
    protected void _roundAttackBeforeHandle(ICardHandlePara para)
    {
        _saveAttackValue = getNumberDigits(para.getAttackUser().getAttack() * getNumber() * 0.01f);
        if (_saveAttackValue >= 0.1f) {
            para.getRoundResult().setSaveAttackValue(_saveAttackValue);
        }
    }

    override
    protected void _roundAttackHandle(ICardHandlePara para) {
        if (_saveAttackValue >= 0.1f) {
            lossBlood(para);

            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getDefenseUser(), para.getCard(), "攻击力保留"+ getNumber() + "%");
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
        }
    }

    protected virtual void lossBlood(ICardHandlePara para) { 

    }

    protected virtual int getNumber() {
        return 20;
    }
}
