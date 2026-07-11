public class SpecialSettle : IAttackSettle
{
    public void settle(ITriggerHandlePara para) {
        this.clear(para.getAttackUser());
        this.clear(para.getDefenseUser());
        para.reset();
    }

    private void clear(IUser user)
    {
        GameEffectMgr.Instance.clearBaseEffectValue(user, BaseEffectType.temporaryArmor);
        GameRunTimeMgr.Instance.clearRunTimeConsumeDefense(user);
        GameRunTimeMgr.Instance.clearRunTimeRoundGetHurt(user);
    }
}