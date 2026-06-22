public class GameDefenseMgr : Singleton<GameDefenseMgr>
{
    public float handle(IUser attackUser, IUser defenseUser, float addValue,bool addMsg = true) {
        if (attackUser == null) {
            return 0;
        }
        if (addValue == 0) {
            return 0;
        }
        float finalValue = attackUser.addDefense(addValue);

        IBaseEffectHandlePara para = new BaseEffectHandleParaObject();
        para.setAttackUser(attackUser);
        para.setDefenseUser(defenseUser);
        para.setEffectType(AdvancedEffectType.addDefense);
        para.setExtralValue(addValue);
        CardMgr.Instance.handle(para);

        if (addMsg) {
            IUICommonPara defensePara = new UICommonParaObject(attackUser, ValueType.defense, addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, defensePara);
        }

        return finalValue;
    }
}
