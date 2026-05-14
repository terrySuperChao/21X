using Pb;

public class AttackSettle: IAttackSettle
{
    public void settle(ITriggerHandlePara para) {
        this.commonAttack(para);
        this.magicAttack(para);
    }

    //普通攻击
    private void commonAttack(ITriggerHandlePara para) {
        IRoundResult roundResult = para.getRoundResult(para.getAttackUser());
        if (roundResult == null)
        {
            return;
        }

        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();

        //暴击伤害加+50%
        float addCrit = 0;
        float number = RandomMgr.Instance.getRangeInt(1, 101) / 100.0f;//[1,100]
        if (number <= attackUser.getExtraInfo().getAddCrit()) {
            addCrit = 0.5f;
        }

        float multATK = attackUser.getExtraInfo().getMultATK();
        float attack = attackUser.getAttack() * (1 + multATK + addCrit);
        if (attack <= 0)
        {
            return;
        }
        attackUser.setAttack(0);
        attackUser.getExtraInfo().setMultATK(-multATK);
        roundResult.addHurtValue(attack);
        
        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.attack, attack, attackUser.getAttack());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);

        float remainAttack = this.getRemainAttack(para, attack);
        GameBloodMgr.Instance.lessBloodHandle(para.getAttackUser(), para.getDefenseUser(), remainAttack);
        
        //反射
        float reflectDMG = defenseUser.getExtraInfo().getReflectDMG();
        if (reflectDMG > 0) {
            defenseUser.getExtraInfo().setReflectDMG(-reflectDMG);
            GameBloodMgr.Instance.lessBloodHandle(para.getDefenseUser(), para.getAttackUser(), reflectDMG);
        }
        
        SwitchParaMgr.Instance.handle(para, () => {
            CardMgr.Instance.handle(para, TriggerEvent.normalAttackAfter);
        }, true);
    }

    //魔法攻击
    public void magicAttack(ITriggerHandlePara para) {
        if (GameBloodMgr.Instance.checkGameOver(para)) {
            return;
        }

        IRoundResult roundResult = para.getRoundResult(para.getAttackUser());
        if (roundResult == null){
            return;
        }

        float attack = 50.0f;
        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();
        if (attackUser.getMagic() < attackUser.getMaxMagic()){
            return;
        }
        attackUser.setMagic(0);
        roundResult.addHurtValue(attack);

        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.magic, attackUser.getMaxMagic(), attackUser.getMagic());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);
        CardMgr.Instance.handle(para, TriggerEvent.magicAttackAfter);

        //减去50血量
        GameBloodMgr.Instance.lessBloodHandle(para.getAttackUser(), para.getDefenseUser(), attack);

        //魔法攻击
        para.setMagicAttack(true);
    }

    //
    private void setDefenseUserBlood(ITriggerHandlePara handlePara, float attack) {
        if (attack <= 0) return;

        IUser defenseUser = handlePara.getDefenseUser();
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

        IUICommonPara attackPara = new UICommonParaObject(defenseUser, ValueType.blood, -bloodValue, defenseUser.getBlood());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
    }

    //
    private float getRemainAttack(ITriggerHandlePara para,float attack) {
        IUser defenseUser = para.getDefenseUser();
        IRoundResult roundResult = para.getRoundResult(para.getUser());
        //
        defenseUser.getExtraInfo().setTemporaryArmor(-attack);
        //
        if (roundResult.getPenetrateValue() != 0)
        {
            return attack;
        }

        //
        float defense = defenseUser.getDefense();
        if (defense <= 0)
        {
            return attack;
        }

        float defenseValue = 0;
        if (attack > defense)
        {
            defenseValue = defense;
            attack -= defense;
            defense = 0;
        }
        else
        {
            defenseValue = attack;
            defense -= attack;
            attack = 0;
        }
        defenseUser.setDefense(defense);
        

        IUICommonPara attackPara = new UICommonParaObject(defenseUser, ValueType.defense, -defenseValue, defenseUser.getDefense());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
        
        return attack;
    }
    
}