using System.Collections.Generic;

public class AttackSettle: IAttackSettle
{
    public void settle(ICardHandlePara handlePara) {
        this.commonAttack(handlePara);
        this.magicAttack(handlePara);
    }

    //ÆÕÍ¨¹¥»÷
    private void commonAttack(ICardHandlePara handlePara) {
        IUser attackUser = handlePara.getAttackUser();
        IUser defenseUser = handlePara.getDefenseUser();
        IRoundResult roundResult = handlePara.getRoundResult();

        float attack = attackUser.getAttack();
        if (attack <= 0)
        {
            return;
        }

        attackUser.setAttack(roundResult.getSaveAttackValue());
        handlePara.setUser(attackUser);
        
        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.attack, attack, attackUser.getAttack());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);
        CardMgr.Instance.handle(handlePara, CardHandleType.roundAttack);

        float remainAttack = this.getRemainAttack(handlePara,attack);
        //¿Û³ýµÄÑªÁ¿
        this.setDefenseUserBlood(handlePara, remainAttack);
    }

    //Ä§·¨¹¥»÷
    public void magicAttack(ICardHandlePara handlePara) {
        IUser attackUser = handlePara.getAttackUser();
        IUser defenseUser = handlePara.getDefenseUser();
        IRoundResult roundResult = handlePara.getRoundResult();
        if (defenseUser.getBlood() < 0 || attackUser.getMagic() < attackUser.getMaxMagic())
        {
            return;
        }

        attackUser.setMagic(roundResult.getSaveMagicValue());
        handlePara.setUser(attackUser);

        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.magic, attackUser.getMaxMagic(), attackUser.getMagic());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);
        CardMgr.Instance.handle(handlePara, CardHandleType.roundMagicAttack);

        //50µÄ¹¥»÷Á¦ ¿Û³ýµÄÑªÁ¿
        this.setDefenseUserBlood(handlePara, 50.0f);
    }

    //¿Û³ý·ÀÓùÕßÑªÁ¿
    private void setDefenseUserBlood(ICardHandlePara handlePara, float attack) {
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
        CardMgr.Instance.handle(handlePara, CardHandleType.roundSubBlood);
    }

    //¿Û³ý·ÀÓùÖ®ºó£¬Ê£Óà¹¥»÷Á¦
    private float getRemainAttack(ICardHandlePara handlePara,float attack) {
        IUser attackUser = handlePara.getAttackUser();
        IUser defenseUser = handlePara.getDefenseUser();
        IRoundResult roundResult = handlePara.getRoundResult();

        //´©´Ì,100%ÉËº¦
        if (roundResult.getPenetrateValue() != 0)
        {
            return attack;
        }

        //»¤¼×Îª0£¬100%ÉËº¦
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
        CardMgr.Instance.handle(handlePara, CardHandleType.roundSubDefense);

        return attack;
    }
    
}