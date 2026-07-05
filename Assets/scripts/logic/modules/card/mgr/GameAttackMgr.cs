public class GameAttackMgr : Singleton<GameAttackMgr>
{
    public float handle(IUser attackUser, float addValue)
    {
        return attackUser.addAttack(addValue);
    }

    public void handle(ITriggerHandlePara para, float addValue) {
        IUser attackUser = para.getAttackUser();
        float finalValue = attackUser.addAttack(addValue);

        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.attack, addValue, finalValue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);

        GameRunTimeMgr.Instance.runTimeCountAttack(para, addValue);
    }
}
