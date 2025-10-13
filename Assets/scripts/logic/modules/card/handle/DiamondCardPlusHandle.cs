//方块大师+
public class DiamondCardPlusHandle : DiamondCardHandle
{
   override
   protected void _roundSubDefenseHandle(ICardHandlePara para)
   {
        float value = getNumberDigits(para.getAttackUser().getAttack() * 0.5f);
        if (value >= 0.1f) {
            float addValue = -value;
            float finalValue = para.getAttackUser().addBlood(addValue);
            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getDefenseUser(), para.getCard(), "反弹伤害" + value);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), ValueType.blood, addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
        }
   }

    override
    protected int getNumber()
    {
        return 5;
    }
}
