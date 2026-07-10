public class GameDefenseMgr : Singleton<GameDefenseMgr>
{
    public float handle(IUser attackUser, float addValue, out float outValue) {
        outValue = addValue;
        return attackUser.addDefense(outValue);
    }

    public void handle(ITriggerHandlePara para, float addValue) {
        float finalValue = para.getAttackUser().addDefense(addValue);
        IUICommonPara defensePara = new UICommonParaObject(para.getAttackUser(), ValueType.defense, addValue, finalValue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, defensePara);

        this.execAdvancedEffectHandle(para,addValue);
    }

    //进阶效果
    public void execAdvancedEffectHandle(ITriggerHandlePara para, float addValue)
    {
        IBaseEffectHandlePara paras = new BaseEffectHandleParaObject();
        paras.setAttackUser(para.getAttackUser());
        paras.setDefenseUser(para.getDefenseUser());
        paras.setEffectType(AdvancedEffectType.addDefense);
        paras.setExtralValue(addValue);
        GameCardMgr.Instance.handle(paras);
    }
}
