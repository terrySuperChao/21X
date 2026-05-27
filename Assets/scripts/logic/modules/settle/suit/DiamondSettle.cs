public class DiamondSettle: SuitSettle
{
    protected override float _getFinalValue(IUser attackUser, float value) {
        return attackUser.addDefense(value);
    }

    protected override bool _matchSuit(PokerSuit suit) { 
        return suit == PokerSuit.diamond; 
    }

    protected override float _getMult() { return 0.5f; }

    protected override void _settle(ITriggerHandlePara para, IHandPokerSuit handPoker) {
        float armorATK = para.getAttackUser().getExtraInfo().getArmorATK();
        if (armorATK > 0){
            GameBloodMgr.Instance.lessBloodHandle(para.getAttackUser(), para.getDefenseUser(), armorATK);
        }

        float addValue = handPoker.getBaseValue() * para.getAttackUser().getExtraInfo().getBonusArmor();
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