using System.Collections.Generic;

public class AttackSettle: IAttackSettle
{
    public void settle(ITriggerHandlePara para) {
        this.commonAttack(para);
        this.magicAttack(para);
    }

    //��ͨ����
    private void commonAttack(ITriggerHandlePara para) {
        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();
        IRoundResult roundResult = para.getRoundResult();

        float attack = attackUser.getAttack();
        if (attack <= 0)
        {
            return;
        }

        attackUser.setAttack(roundResult.getSaveAttackValue());
        para.setUser(attackUser);
        
        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.attack, attack, attackUser.getAttack());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);
        CardMgr.Instance.handle(para, CardHandleType.roundAttack);

        float remainAttack = this.getRemainAttack(para, attack);
        //�۳���Ѫ��
        this.setDefenseUserBlood(para, remainAttack);
    }

    //ħ������
    public void magicAttack(ITriggerHandlePara para) {
        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();
        IRoundResult roundResult = para.getRoundResult();
        if (defenseUser.getBlood() < 0 || attackUser.getMagic() < attackUser.getMaxMagic())
        {
            return;
        }

        attackUser.setMagic(roundResult.getSaveMagicValue());
        para.setUser(attackUser);

        IUICommonPara attackPara = new UICommonParaObject(attackUser, ValueType.magic, attackUser.getMaxMagic(), attackUser.getMagic());
        GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);
        CardMgr.Instance.handle(para, CardHandleType.roundMagicAttack);

        //50�Ĺ����� �۳���Ѫ��
        this.setDefenseUserBlood(para, 50.0f);
    }

    //�۳�������Ѫ��
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
        CardMgr.Instance.handle(handlePara, CardHandleType.roundSubBlood);
    }

    //�۳�����֮��ʣ�๥����
    private float getRemainAttack(ITriggerHandlePara para,float attack) {
        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();
        IRoundResult roundResult = para.getRoundResult();

        //����,100%�˺�
        if (roundResult.getPenetrateValue() != 0)
        {
            return attack;
        }

        //����Ϊ0��100%�˺�
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
        CardMgr.Instance.handle(para, CardHandleType.roundSubDefense);

        return attack;
    }
    
}