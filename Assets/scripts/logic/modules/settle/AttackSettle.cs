public class AttackSettle: IAttackSettle
{
    public void settle(ITriggerHandlePara para) {
        this.commonAttack(para);
        this.magicAttack(para);
    }

    //普通攻击
    private void commonAttack(ITriggerHandlePara para) {
        if (GameBloodMgr.Instance.checkGameOver(para)){
            return;
        }

        if (para.getAttackUser().getAttack() <= 0) {
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

        //保留
        float retainATK = attackUser.getExtraInfo().getRetainATK();
        if (addCrit > 0){
            attackUser.getExtraInfo().clearRetainATK();
        }else {
            retainATK = 0.0f;
        }

        //终结技
        float execute = attackUser.getExtraInfo().getExecute();
        float percent = defenseUser.getBlood() / defenseUser.getMaxBlood() * 100.0f;
        if (execute > percent){
            attack = defenseUser.getBlood();
            attackUser.getExtraInfo().clearExecute();
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
            float immunityDeBuff = attackUser.getExtraInfo().getImmunityDeBuff();
            if (immunityDeBuff == 0){
                GameBloodMgr.Instance.lessBloodHandle(defenseUser, attackUser, addBleeding);
            }else {
                IUIFlyFontPara uiPara = new UIFlyFontParaObject(defenseUser, BuffType.immunityDeBuff, "免疫的流血");
                GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara);
            }
            attackUser.getExtraInfo().setAddBleeding(-1);
        }

        //反弹
        float reflectDMG = defenseUser.getExtraInfo().getReflectDMG();
        if (reflectDMG > 0) {
            GameBloodMgr.Instance.lessBloodHandle(defenseUser, attackUser, reflectDMG);
        }

        //反弹百分比
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

        //清空状态
        

        //普通攻击两次
        float doubleProc = attackUser.getExtraInfo().getDoubleProc();
        if (doubleProc > 0) {
            attackUser.getExtraInfo().clearDoubleProc();
            this.commonAttack(para);
        }
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
        attackUser.setMagic(0);
        
        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.magic, attackUser.getMaxMagic(), attackUser.getMagic());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);

        float magicImmunity = defenseUser.getExtraInfo().getMagicImmunity();
        if (magicImmunity > 0)
        {
            defenseUser.getExtraInfo().clearMagicImmunity();

            IUIFlyFontPara uiPara = new UIFlyFontParaObject(defenseUser, BuffType.magicImmunity, "免疫的护盾");
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara);
        }
        else
        {
            //减去50血量
            float attack = 50.0f;
            float skillDamageUp = attackUser.getExtraInfo().getSkillDamageUp();
            float remainAttack = GameUtils.getNumberDigits(attack * (1 + skillDamageUp));
            attackUser.getExtraInfo().setRtHurtValue(remainAttack);

            GameBloodMgr.Instance.lessBloodHandle(para.getAttackUser(), para.getDefenseUser(), remainAttack);
        }

        //魔法攻击后
        CardMgr.Instance.handle(para, TriggerEvent.magicAttackAfter);

        //单次造成伤害
        CardMgr.Instance.handle(para, TriggerEvent.roundOther);

        //魔法攻击
        para.setMagicAttack(true);
    }

    //
    private float getRemainAttack(ITriggerHandlePara para,float attack) {
        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();
        defenseUser.getExtraInfo().setTemporaryArmor(-attack);

        //忽略护甲
        if (attackUser.getExtraInfo().getIgnoreArmor() > 0) {
            attackUser.getExtraInfo().clearIgnoreArmor();
            return attack;
        }

        float rtFreezeArmorValue = defenseUser.getExtraInfo().getRtFreezeArmorValue();
        if (rtFreezeArmorValue > 0) {
            defenseUser.addDefense(rtFreezeArmorValue);
            defenseUser.getExtraInfo().clearRtFreezeArmorValue();
            IUICommonPara defensePara = new UICommonParaObject(defenseUser, ValueType.defense, rtFreezeArmorValue, defenseUser.getDefense());
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, defensePara);
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
        float freezeArmor = defenseUser.getExtraInfo().getFreezeArmor();
        if (freezeArmor > 0) {
            defenseUser.getExtraInfo().clearFreezeArmor();
            defenseUser.getExtraInfo().setRtFreezeArmorValue(defenseUser.getDefense() - defense);
        }
        defenseUser.setDefense(defense);
        
        IUICommonPara attackPara = new UICommonParaObject(defenseUser, ValueType.defense, -defenseValue, defenseUser.getDefense());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
        
        return attack;
    }
    
}