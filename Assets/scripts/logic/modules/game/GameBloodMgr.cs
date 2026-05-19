using System.Collections.Generic;

public class GameBloodMgr : Singleton<GameBloodMgr>
{
    public void addBloodHandle(List<IUser> users)
    {
        for (int i = 0; i < users.Count; i++)
        {
            IUser user = users[i];
            float addValue = user.getExtraInfo().getHealOverTime();
            if (addValue > 0)
            {
                this.addBloodHandle(user,addValue);
            }
        }
    }

    public void addBloodHandle(IUser user,float addValue) {
        if (user != null) {
            user.addBlood(addValue);
            IUICommonPara attackPara = new UICommonParaObject(user, ValueType.blood, addValue, user.getBlood());
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
        }
    }

    public void lessBloodHandle(IUser attackUser, IUser defenseUser, float attack)
    {
        if (attack <= 0) return;

        float blood = defenseUser.getBlood();
        float bloodValue = 0;

        attack = GameUtils.getNumberDigits(attack);

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

        IUICommonPara attackPara = new UICommonParaObject(defenseUser, ValueType.blood, -bloodValue, defenseUser.getBlood());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);

        //使用真实血量
        float addValue = bloodValue * attackUser.getExtraInfo().getLifeSteal();
        if (addValue > 0) {
            attackUser.addBlood(addValue);
            IUICommonPara bloodPara = new UICommonParaObject(attackUser, ValueType.blood, addValue, attackUser.getBlood());
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, bloodPara);
        }
        
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
