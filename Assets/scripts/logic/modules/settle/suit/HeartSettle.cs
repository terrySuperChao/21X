using static UnityEngine.UIElements.UxmlAttributeDescription;

public class HeartSettle : SuitSettle
{
    override
    protected float _getFinalValue(IUser attackUser, float value) {
        return attackUser.addBlood(value);
    }

    override
    protected bool _matchSuit(int suit) { 
        return suit == (int)PokerSuit.heart; 
    }

    override
    protected float _getMult() { return 0.5f; }

    override
    protected void _settle(ITriggerHandlePara para, IPoker poker, int baseValue)
    {
        float addValue = baseValue * para.getAttackUser().getExtraInfo().getHealToMP();
        if (addValue > 0)
        {
            para.getAttackUser().addMagic(addValue);
            IUICommonPara attackPara = new UICommonParaObject(para.getAttackUser(), ValueType.magic, addValue, para.getAttackUser().getMagic());
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
        }
    }
}