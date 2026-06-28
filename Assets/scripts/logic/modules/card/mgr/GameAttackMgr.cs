public class GameAttackMgr : Singleton<GameAttackMgr>
{

    public float handle(IUser attackUser, IUser defenseUser, float addValue, bool addMsg = true) {
        if (attackUser == null) {
            return 0;
        }
        float finalValue = attackUser.addAttack(addValue);

        if (addMsg) { 
            IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.attack, addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
        }

        return finalValue;
    }
}
