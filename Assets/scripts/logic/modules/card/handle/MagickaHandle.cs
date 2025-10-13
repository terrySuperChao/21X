//魔能外溢
public class MagickaHandle : CardHandleObject
{
    override
    protected void _roundAddValueBeforeHandle(ICardHandlePara para)
    { 
        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        if (suit == PokerSuit.club)
        { //梅花
            _roundAddMagicHandle(para);
        }
    }

    override
    protected void _roundAddMagicHandle(ICardHandlePara para)
    {
        int number = getNumber();
        int maxNumber = getMaxNumber();
        float value = RandomMgr.Instance.getRangeInt(number, maxNumber);
        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "魔法伤害+" + value);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

        float addValue = -value;
        float finalVlaue = para.getDefenseUser().addBlood(addValue);
        IUICommonPara uiPara2 = new UICommonParaObject(para.getDefenseUser(), ValueType.blood, addValue, finalVlaue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
    }

    protected virtual int getNumber()
    {
        return 1;
    }

    protected virtual int getMaxNumber()
    {
        return 3;
    }
}
