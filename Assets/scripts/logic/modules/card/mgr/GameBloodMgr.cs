using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;

public class GameBloodMgr : Singleton<GameBloodMgr>
{
    //加血
    public void handle(List<IUser> users)
    {
        IUser attackUser = users.Find(user => user.isNpc());
        IUser defenseUser = users.Find(user => !user.isNpc());

        //npc
        IBaseEffectHandlePara para = new BaseEffectHandleParaObject();
        para.setAttackUser(attackUser);
        para.setDefenseUser(defenseUser);
        para.setEffectType(AdvancedEffectType.roundStartAddBlood);
        para.setExtralValue(0);
        GameCardMgr.Instance.handle(para);

        para.setAttackUser(defenseUser);
        para.setDefenseUser(attackUser);
        GameCardMgr.Instance.handle(para);
    }

    //加血
    public float handle(IUser user,float addValue, bool addMsg = true) {
        if (user == null) {
            return 0;
        }

        if(addValue == 0){
            return 0;
        }

        float maxValue = user.getBlood() + addValue;
        float value = user.addBlood(addValue);
        if (addMsg) {
            IUICommonPara bloodPara = new UICommonParaObject(user, ValueType.blood, addValue, user.getBlood());
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, bloodPara);
        }
        
        //保存溢出的血量
        IBaseEffectHandlePara para = new BaseEffectHandleParaObject();
        para.setAttackUser(user);
        para.setDefenseUser(null);
        para.setEffectType(AdvancedEffectType.overflowBloodValue);
        para.setExtralValue(maxValue - user.getMaxBlood());
        GameCardMgr.Instance.handle(para);
       
        return value;
    }

    //扣血
    public void handle(IUser attackUser, IUser defenseUser, float attack)
    {
        if (attack <= 0) return;

        float blood = defenseUser.getBlood();
        float bloodValue = 0;

        if (attack > blood)
        {
            bloodValue = blood;
            blood = 0;
        }
        else
        {
            bloodValue = attack;
            blood -= attack;
        }
        defenseUser.setBlood(blood);

        //伤害
        attackUser.getExtraInfo().setRtHurtValue(bloodValue);

        IUICommonPara attackPara = new UICommonParaObject(defenseUser, ValueType.blood, -bloodValue, defenseUser.getBlood());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);

        //攻击方
        IBaseEffectHandlePara para = new BaseEffectHandleParaObject();
        para.setAttackUser(attackUser);
        para.setDefenseUser(defenseUser);
        para.setEffectType(AdvancedEffectType.enemyLessBlood);
        para.setExtralValue(bloodValue);
        GameCardMgr.Instance.handle(para);

        para.setAttackUser(defenseUser);
        para.setDefenseUser(attackUser);
        para.setEffectType(AdvancedEffectType.selfLessBlood);
        GameCardMgr.Instance.handle(para);

        if (blood <= 0)
        {
            GameMessage.Instance.addMsg(GameConst.FIGHTFLOWSTATE, FightFlowState.fightOver);
            GameMessage.Instance.addMsg(GameConst.GAMESETTLE, attackUser);
            GameMessage.Instance.addMsg(GameConst.GAMEOVER);
        }
    }

    public bool checkGameOver(ITriggerHandlePara para) {
        return para.getAttackUser().getBlood() <= 0 ||
               para.getDefenseUser().getBlood() <= 0;
    }

    
}
