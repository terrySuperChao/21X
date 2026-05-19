using Pb;

public class AttackSettle: IAttackSettle
{
    public void settle(ITriggerHandlePara para) {
        float attackNum = 1;
        float doubleProc = para.getAttackUser().getExtraInfo().getDoubleProc();
        for (int i = 0; i < attackNum + doubleProc; i++) {
            this.commonAttack(para);
        }
        this.magicAttack(para);
    }

    //普通攻击
    private void commonAttack(ITriggerHandlePara para) {
        if (GameBloodMgr.Instance.checkGameOver(para)){
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
        float attackN = attackUser.getAttack() * (1 + multATK + addCrit);
        float attack = GameUtils.getNumberDigits(attackN);
        float retainATK = attackUser.getExtraInfo().getRetainATK();
        //终结技
        float execute = attackUser.getExtraInfo().getExecute();
        float percent = defenseUser.getBlood() / defenseUser.getMaxBlood() * 100;
        if (execute > percent)
        {
            attack = defenseUser.getBlood();
        }

        if (attack <= 0){
            return;
        }
        attackUser.setAttack(GameUtils.getNumberDigits(attack * retainATK));
        attackUser.getExtraInfo().setRtHurtValue(attack);
        
        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.attack, attack, attackUser.getAttack());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);

        float remainAttack = this.getRemainAttack(para, attack);
        GameBloodMgr.Instance.lessBloodHandle(para.getAttackUser(), para.getDefenseUser(), remainAttack);

        //流血
        float addBleeding = attackUser.getExtraInfo().getAddBleeding();
        if (addBleeding > 0){
            GameBloodMgr.Instance.lessBloodHandle(defenseUser, attackUser, addBleeding);
            attackUser.getExtraInfo().setAddBleeding(-1);
        }

        //反射
        float reflectDMG = defenseUser.getExtraInfo().getReflectDMG();
        if (reflectDMG > 0) {
            defenseUser.getExtraInfo().clearReflectDMG();
            GameBloodMgr.Instance.lessBloodHandle(defenseUser, attackUser, reflectDMG);
        }

        //反弹
        float reflectPercent = defenseUser.getExtraInfo().getReflectPercent();
        if (reflectPercent > 0){
            defenseUser.getExtraInfo().clearReflectPercent();
            GameBloodMgr.Instance.lessBloodHandle(defenseUser, attackUser, attack * reflectPercent);
        }

        //单次造成伤害
        CardMgr.Instance.handle(para, TriggerEvent.roundOther);

        //普通攻击后
        SwitchParaMgr.Instance.handle(para, () => {
            CardMgr.Instance.handle(para, TriggerEvent.normalAttackAfter);
        }, true);  
    }

    //魔法攻击
    public void magicAttack(ITriggerHandlePara para) {
        if (GameBloodMgr.Instance.checkGameOver(para)) {
            return;
        }

        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();
        if (attackUser.getMagic() < attackUser.getMaxMagic()){
            return;
        }

        float attack = 50.0f;
        float skillDamageUp = attackUser.getExtraInfo().getSkillDamageUp();
        float remainAttack = GameUtils.getNumberDigits(attack * (1 + skillDamageUp));

        attackUser.setMagic(0);
        attackUser.getExtraInfo().setRtHurtValue(remainAttack);
        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.magic, attackUser.getMaxMagic(), attackUser.getMagic());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);

        //魔法攻击后
        CardMgr.Instance.handle(para, TriggerEvent.magicAttackAfter);

        //单次造成伤害
        CardMgr.Instance.handle(para, TriggerEvent.roundOther);

        float magicImmunity = defenseUser.getExtraInfo().getMagicImmunity();
        if (magicImmunity > 0)
        {
            defenseUser.getExtraInfo().clearMagicImmunity();
            IUIFlyFontPara uiPara = new UIFlyFontParaObject(defenseUser, para.getAssembleCard(), "免疫的护盾");
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara);
        }
        else
        {
            //减去50血量
            GameBloodMgr.Instance.lessBloodHandle(para.getAttackUser(), para.getDefenseUser(), remainAttack);
        }
       
        //魔法攻击
        para.setMagicAttack(true);
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

        if (para.getAttackUser().getExtraInfo().getIgnoreArmor() > 0) {
            para.getAttackUser().getExtraInfo().clearIgnoreArmor();
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