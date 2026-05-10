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
        float attack = attackUser.getAttack();
        if (attack <= 0)
        {
            return;
        }
        roundResult.addHurtValue(attack);
        
        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.attack, attack, attackUser.getAttack());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);

        float remainAttack = this.getRemainAttack(para, attack);
        this.setDefenseUserBlood(para, remainAttack);

        SwitchParaMgr.Instance.handle(para, () => {
            CardMgr.Instance.handle(para, TriggerEvent.normalAttackAfter);
        }, true);
    }

    //魔法攻击
    public void magicAttack(ITriggerHandlePara para) {
        IRoundResult roundResult = para.getRoundResult(para.getAttackUser());
        if (roundResult == null)
        {
            return;
        }

        //添加魔法
        AssetInfo info = FightPokerMgr.Instance.getAssetInfo(para.getAttackUser());
        if (info == null)
        {
            return;
        }
        roundResult.addMagicValue(para.getAttackUser().getMagic() - info.Magic);

        float attack = 50.0f;
        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();
        if (defenseUser.getBlood() < 0 || attackUser.getMagic() < attackUser.getMaxMagic()){
            return;
        }
        roundResult.addHurtValue(attack);

        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.magic, attackUser.getMaxMagic(), attackUser.getMagic());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);
        CardMgr.Instance.handle(para, TriggerEvent.magicAttackAfter);

        //减去50血量
        this.setDefenseUserBlood(para, attack);

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
        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();
        IRoundResult roundResult = para.getRoundResult(para.getUser());

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