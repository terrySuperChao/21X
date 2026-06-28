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
        CardMgr.Instance.handle(para);

        para.setAttackUser(defenseUser);
        para.setDefenseUser(attackUser);
        CardMgr.Instance.handle(para);
    }

    public float handle(IUser attackUser, IUser defenseUser, float addValue, bool addMsg = true) {
        if (attackUser == null) {
            return 0;
        }

        float magicDouble = CardMgr.Instance.getBaseEffectValue(attackUser, BaseEffectType.magicDouble);
        if (magicDouble > 0) {
            addValue *= magicDouble;
        }
        float finalValue = attackUser.addMagic(addValue);

        IBaseEffectHandlePara para = new BaseEffectHandleParaObject();
        para.setAttackUser(attackUser);
        para.setDefenseUser(defenseUser);
        para.setEffectType(AdvancedEffectType.addMagic);
        para.setExtralValue(addValue);
        CardMgr.Instance.handle(para);

        if (addMsg){
            IUICommonPara magicPara = new UICommonParaObject(para.getAttackUser(), ValueType.magic, addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, magicPara);
        }

        return finalValue;
    }

    public void handle(IUser attackUser, float addValue)
    {
        if (attackUser == null){
            return;
        }
        attackUser.addMaxMagic(addValue);
      
        IUICommonPara magicPara = new UICommonParaObject(attackUser, ValueType.maxMagic, attackUser.getMagic(), attackUser.getMaxMagic());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, magicPara);
    }
}
