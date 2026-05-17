public class DiamondSettle: SuitSettle
{
    override
    protected float _getFinalValue(IUser attackUser, float value) {
        return attackUser.addDefense(value);
    }

    override
    protected bool _matchSuit(int suit) { 
        return suit == (int)PokerSuit.diamond; 
    }

    override
    protected float _getMult() { return 0.5f; }

    override
    protected void _settle(ITriggerHandlePara para, IPoker poker, int baseValue) {
        float armorATK = para.getAttackUser().getExtraInfo().getArmorATK();
        if (armorATK > 0){
            GameBloodMgr.Instance.lessBloodHandle(para.getAttackUser(), para.getDefenseUser(), armorATK);
        }

        float addValue = baseValue * para.getAttackUser().getExtraInfo().getBonusArmor();
        if (addValue > 0) {
            para.getAttackUser().addDefense(addValue);
            IUICommonPara attackPara = new UICommonParaObject(para.getAttackUser(), ValueType.defense, addValue, para.getAttackUser().getDefense());
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);

            if (armorATK > 0){
                GameBloodMgr.Instance.lessBloodHandle(para.getAttackUser(), para.getDefenseUser(), armorATK);
            }
        }        
    }
}