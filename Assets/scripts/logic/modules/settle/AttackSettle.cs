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
        if (number <= GameEffectMgr.Instance.getBaseEffectValue(attackUser,BaseEffectType.addCrit)) {
            addCrit = 0.5f;
        }

        float multATK = GameEffectMgr.Instance.getBaseEffectValue(attackUser, BaseEffectType.multATK);
        float attack = attackUser.getAttack() * (1 + multATK + addCrit);

        //保留
        float retainATK = GameEffectMgr.Instance.getBaseEffectValue(attackUser, BaseEffectType.retainATK);
        attackUser.setAttack(attack * retainATK);
       
        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.attack, attack, attackUser.getAttack());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);

        float remainAttack = this.getRemainAttack(para, attack);
        GameBloodMgr.Instance.handle(para, remainAttack);
        GameRunTimeMgr.Instance.runTimeConsumeDefense(para.getDefenseUser());

        //反弹
        float reflectDMG = GameEffectMgr.Instance.getBaseEffectValue(defenseUser,BaseEffectType.reflectDMG);
        if (reflectDMG > 0) {
            SwitchParaMgr.Instance.handle(para, () => {
                GameBloodMgr.Instance.handle(para, reflectDMG);
            }, true);
        }

        //普通攻击后
        SwitchParaMgr.Instance.handle(para, () => {
            GameCardMgr.Instance.handle(para, TriggerEvent.POST_BASIC_ATTACK);
        }, false);

        //清空
        GameEffectMgr.Instance.clearBaseEffectValue(attackUser, BaseEffectType.multATK);
        GameEffectMgr.Instance.clearBaseEffectValue(attackUser, BaseEffectType.retainATK);
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
        float skillDamageUp = GameEffectMgr.Instance.getBaseEffectValue(attackUser, BaseEffectType.skillDamageUp);
        float remainAttack = attack * (1 + skillDamageUp);
        attackUser.getExtraInfo().setMagicAttack(true);
        
        GameBloodMgr.Instance.handle(para, remainAttack);
        
        //魔法攻击后
        GameCardMgr.Instance.handle(para, TriggerEvent.POST_MAIN_SKILL);

        //单次造成伤害
        GameCardMgr.Instance.handle(para, TriggerEvent.CUSTOM_EVENT, GameCardConst.TriggerEffectId1023, remainAttack);

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
        float ignoreArmor = GameEffectMgr.Instance.getBaseEffectValue(attackUser, BaseEffectType.ignoreArmor);
        if (ignoreArmor > 0) {
            GameEffectMgr.Instance.clearBaseEffectValue(attackUser, BaseEffectType.ignoreArmor);
            return attack;
        }

        //扣除临时护甲
        attack = GameEffectMgr.Instance.subtractBaseEffectValue(defenseUser, BaseEffectType.temporaryArmor, attack);
        if (attack <= 0) {
            return attack;
        }
        
        //护甲
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

        SwitchParaMgr.Instance.handle(para, () => {
            GameDefenseMgr.Instance.handle(para, -defenseValue);
        }, true);
         
        return attack;
    }
}