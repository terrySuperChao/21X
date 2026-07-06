public class SpecialSettle : IAttackSettle
{
    public void settle(ITriggerHandlePara para) {
        this.removeTemporaryArmor(para.getAttackUser(),para.getDefenseUser());
        this.removeTemporaryArmor(para.getDefenseUser(),para.getAttackUser());
        GameRunTimeMgr.Instance.clearRunTimeConsumeDefense(para.getDefenseUser());
        para.reset();
    }

    private void removeTemporaryArmor(IUser attackUser,IUser defenseUser) {
        float addValue = CardMgr.Instance.getBaseEffectValue(attackUser, BaseEffectType.temporaryArmor);
        if (addValue > 0) {
            GameDefenseMgr.Instance.handle(attackUser, defenseUser, -addValue);
            CardMgr.Instance.clearBaseEffectValue(attackUser, BaseEffectType.temporaryArmor);
        }
    }
}