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
        if (number <= GameCardMgr.Instance.getBaseEffectValue(attackUser,BaseEffectType.addCrit)) {
            addCrit = 0.5f;
        }

        float multATK = GameCardMgr.Instance.getBaseEffectValue(attackUser, BaseEffectType.multATK);
        float attack = attackUser.getAttack() * (1 + multATK + addCrit);

        //保留
        float retainATK = GameCardMgr.Instance.getBaseEffectValue(attackUser, BaseEffectType.retainATK);
        attackUser.setAttack(attack * retainATK);
        attackUser.getExtraInfo().setRtHurtValue(attack);
        
        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.attack, attack, attackUser.getAttack());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);

        float remainAttack = this.getRemainAttack(para, attack);
        GameRunTimeMgr.Instance.runTimeConsumeDefense(para.getDefenseUser());
        GameBloodMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), remainAttack);

        //反弹
        float reflectDMG = GameCardMgr.Instance.getBaseEffectValue(defenseUser,BaseEffectType.reflectDMG);
        if (reflectDMG > 0) {
            GameBloodMgr.Instance.handle(defenseUser, attackUser, reflectDMG);
        }

        //单次造成伤害
        GameCardMgr.Instance.handle(para, TriggerEvent.CUSTOM_EVENT);

        //普通攻击后
        SwitchParaMgr.Instance.handle(para, () => {
            GameCardMgr.Instance.handle(para, TriggerEvent.POST_BASIC_ATTACK);
        }, true);

        //清空
        GameCardMgr.Instance.clearBaseEffectValue(attackUser, BaseEffectType.multATK);
        GameCardMgr.Instance.clearBaseEffectValue(attackUser, BaseEffectType.retainATK);
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

        //减去50血量
        float attack = 50.0f;
        float skillDamageUp = GameCardMgr.Instance.getBaseEffectValue(attackUser, BaseEffectType.skillDamageUp);
        float remainAttack = attack * (1 + skillDamageUp);
        attackUser.getExtraInfo().setRtHurtValue(remainAttack);
        attackUser.getExtraInfo().setMagicAttack(true);
        
        GameBloodMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), remainAttack);
        
        //魔法攻击后
        GameCardMgr.Instance.handle(para, TriggerEvent.POST_MAIN_SKILL);

        //单次造成伤害
        GameCardMgr.Instance.handle(para, TriggerEvent.CUSTOM_EVENT);

        //释放
        IBaseEffectHandlePara baseEffectHandlePara = new BaseEffectHandleParaObject();
        baseEffectHandlePara.setAttackUser(attackUser);
        baseEffectHandlePara.setDefenseUser(defenseUser);
        baseEffectHandlePara.setEffectType(AdvancedEffectType.releaseMagic);
        baseEffectHandlePara.setExtralValue(0);
        GameCardMgr.Instance.handle(baseEffectHandlePara);
    }

    //
    private float getRemainAttack(ITriggerHandlePara para,float attack) {
        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();
        
        //忽略护甲
        IBaseEffectData advancedEffectData = attackUser.getExtraInfo().getBaseEffectData(GameCardConst.advancedEffectId3003);
        if (advancedEffectData.isState()) {
            advancedEffectData.setState(0);
            return attack;
        }

        float rtFreezeArmorValue = defenseUser.getExtraInfo().getRtFreezeArmorValue();
        if (rtFreezeArmorValue > 0) {
            GameDefenseMgr.Instance.handle(defenseUser, attackUser, rtFreezeArmorValue);
            defenseUser.getExtraInfo().clearRtFreezeArmorValue();   
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
        }
        else
        {
            defenseValue = attack;
            attack = 0;
        }
        GameDefenseMgr.Instance.handle(defenseUser, attackUser, -defenseValue);
         
        return attack;
    }
    
}