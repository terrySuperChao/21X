using System.Collections.Generic;

public class GameMagicMgr : Singleton<GameMagicMgr>
{
    public void handle(List<IUser> users)
    {
        IUser attackUser = users.Find(user => user.isNpc());
        IUser defenseUser = users.Find(user => !user.isNpc());

        //npc
        IBaseEffectHandlePara para = new BaseEffectHandleParaObject();
        para.setAttackUser(attackUser);
        para.setDefenseUser(defenseUser);
        para.setEffectType(AdvancedEffectType.roundStartAddMagic);
        para.setExtralValue(0);
        GameCardMgr.Instance.handle(para);

        para.setAttackUser(defenseUser);
        para.setDefenseUser(attackUser);
        GameCardMgr.Instance.handle(para);
    }

    public void handle(IUser attackUser, float addValue)
    {
        float finalValue = attackUser.addMagic(addValue);
        this.addMagicMessage(attackUser, ValueType.magic, addValue, finalValue);
    }

    public float handle(IUser attackUser, float addValue,out float outValue) {
        float magicDouble = GameCardMgr.Instance.getBaseEffectValue(attackUser, BaseEffectType.magicDouble);
        if (magicDouble > 0)
        {
            outValue = addValue * magicDouble;
        }
        else {
            outValue = addValue;
        }
        return attackUser.addMagic(outValue);
    }

    public void handle(ITriggerHandlePara para, float addValue) {
        float outVaule = 0;
        float finalValue = this.handle(para.getAttackUser(), addValue, out outVaule);

        this.addMagicMessage(para.getAttackUser(), ValueType.magic, outVaule, finalValue);
        this.execAdvancedEffectHandle(para, outVaule);
    }

    //进阶效果
    public void execAdvancedEffectHandle(ITriggerHandlePara para, float addValue)
    {
        IBaseEffectHandlePara paras = new BaseEffectHandleParaObject();
        paras.setAttackUser(para.getAttackUser());
        paras.setDefenseUser(para.getDefenseUser());
        paras.setEffectType(AdvancedEffectType.addMagic);
        paras.setExtralValue(addValue);
        GameCardMgr.Instance.handle(paras);

        //单次获得法力值
        GameCardMgr.Instance.handle(para, TriggerEvent.CUSTOM_EVENT,GameCardConst.TriggerEffectId1033,addValue);
    }

    public void addmaxMagicHandle(IUser attackUser, float addValue)
    {
        if (attackUser == null){
            return;
        }
        attackUser.addMaxMagic(addValue);
        this.addMagicMessage(attackUser, ValueType.maxMagic, attackUser.getMagic(), attackUser.getMaxMagic());
    }

    private void addMagicMessage(IUser user, ValueType type, float value, float finalValue) {
        IUICommonPara magicPara = new UICommonParaObject(user, type, value, finalValue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, magicPara);
    }
}
