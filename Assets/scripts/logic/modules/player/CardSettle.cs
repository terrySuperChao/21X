//≈∆∂—
using System.Collections.Generic;

public class CardSettle : IGameSettle
{
    public bool gameSettle(IGameSettlePara para) {
        int winIndex = para.getWinIndex();
        if (winIndex == -1)
        {
            cardHandleTypeHandle(para.getUsers(), false, CardHandleType.roundAttackBegin);
            cardHandleTypeHandle(para.getUsers(), true, CardHandleType.roundAttackBegin);
            return false;
        }
        else { 
            List<IUser> users = para.getUsers();
            IUser attackUser = users[winIndex];
            IUser defenseUser = users[winIndex == 0 ? 1 : 0];

            IRoundResult roundResult = new RoundResultObject();
            ICardHandlePara handlePara = new CardHandleParaObject();
            handlePara.setUser(attackUser);
            handlePara.setAttackUser(attackUser);
            handlePara.setDefenseUser(defenseUser);
            handlePara.setRoundResult(roundResult);

            CardMgr.Instance.handle(handlePara, CardHandleType.roundBegin);
        
            //ÃÌº”÷µ
            List<IPoker> pokers = HandPokerMgr.Instance.getHandPoker(attackUser);
            List<int> values = getPokerValue(pokers);
            for (int j = 0; j < pokers.Count; j++)
            {
                IPoker poker = pokers[j];
                float addValue = values[j] * roundResult.getAttributeMult();
                float finalValue = 0;
                ValueType type = GameConst.SuitTransformValueType((PokerSuit)poker.getSuit());
                switch (type)
                {
                    case ValueType.defense: // ∑Ω
                        addValue *= 0.5f;
                        finalValue = attackUser.addDefense(addValue);
                        break;
                    case ValueType.blood: // ∫Ï
                        addValue *= 0.5f;
                        finalValue = attackUser.addBlood(addValue);
                        break;
                    case ValueType.attack: // ∫⁄
                        addValue *= 1.0f;
                        finalValue = attackUser.addAttack(addValue);
                        break;
                    case ValueType.magic: // √∑
                        addValue *= 1.0f;
                        finalValue = attackUser.addMagic(addValue);
                        break;
                    default:
                        break;
                }
                IUIPokerPara pokerPara = new UIPokerPara(attackUser, poker, addValue, finalValue, para.isBackJock());
                GameMessage.Instance.addMsg(GameConst.ADDPOKERVALUE, pokerPara);

                handlePara.setPoker(poker);
                handlePara.setBaseValue(addValue);
                CardMgr.Instance.handle(handlePara, CardHandleType.roundAddValueBefore);
                CardMgr.Instance.handle(handlePara, CardHandleType.roundAddValue);
            }

            //π•ª˜Ω·À„
            handlePara.setUser(attackUser);
            CardMgr.Instance.handle(handlePara, CardHandleType.roundAttackBegin);

            handlePara.setUser(defenseUser);
            CardMgr.Instance.handle(handlePara, CardHandleType.roundAttackBegin);

            float attack = attackUser.getAttack();
            attackUser.setAttack(roundResult.getSaveAttackValue());
            if (attack > 0) {
                IUICommonPara attackPara0 = new UICommonParaObject(attackUser, ValueType.attack, attack, attackUser.getAttack());
                GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara0);

                handlePara.setUser(attackUser);
                CardMgr.Instance.handle(handlePara, CardHandleType.roundAttack);

                if (roundResult.getPenetrateValue() == 0)
                {
                    float defense = defenseUser.getDefense();
                    if (defense > 0) {
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

                        IUICommonPara attackPara1 = new UICommonParaObject(defenseUser, ValueType.defense, -defenseValue, defenseUser.getDefense());
                        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara1);

                        handlePara.setUser(defenseUser);
                        CardMgr.Instance.handle(handlePara, CardHandleType.roundSubDefense);
                        handlePara.setUser(attackUser);
                    }
                }

                float blood = defenseUser.getBlood();
                if (attack > 0) {
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
                    IUICommonPara attackPara1 = new UICommonParaObject(defenseUser, ValueType.blood, -bloodValue, defenseUser.getBlood());
                    GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara1);
                    CardMgr.Instance.handle(handlePara, CardHandleType.roundSubBlood);
                }
            }

            if (defenseUser.getBlood() > 0 && attackUser.getMagic() >= attackUser.getMaxMagic())
            {
                attack = 50;
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

                attackUser.setMagic(roundResult.getSaveMagicValue());
                IUICommonPara attackPara1 = new UICommonParaObject(attackUser, ValueType.magic, attackUser.getMaxMagic(), attackUser.getMagic());
                GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara1);
                CardMgr.Instance.handle(handlePara, CardHandleType.roundMagicAttack);

                IUICommonPara attackPara2 = new UICommonParaObject(defenseUser, ValueType.blood, -bloodValue, defenseUser.getBlood());
                GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara2);
                CardMgr.Instance.handle(handlePara, CardHandleType.roundSubBlood);
            }
            CardMgr.Instance.handle(handlePara, CardHandleType.roundAttackAfter);

            handlePara.setUser(attackUser);
            CardMgr.Instance.handle(handlePara, CardHandleType.roundEnd);

            handlePara.setUser(defenseUser);
            CardMgr.Instance.handle(handlePara, CardHandleType.roundEnd);

            return defenseUser.getBlood() <= 0 || attackUser.getBlood() <= 0;
        }
    }

    public List<int> getPokerValue(List<IPoker> pokers)
    {
        List<int> values = new List<int>();
        List<IPoker> APokers = new List<IPoker>();
        for (int i = 0; i < pokers.Count; i++)
        {
            if (pokers[i].getRank() == 14){
                APokers.Add(pokers[i]);
                values.Add(0);
            }else if (pokers[i].getRank() == 10 ||
                      pokers[i].getRank() == 11 ||
                      pokers[i].getRank() == 12 ||
                      pokers[i].getRank() == 13){
                values.Add(10);
            }else{
                values.Add(pokers[i].getRank());
            }
        }

        int remainPoint = 21;
        for (int j = 0; j < values.Count; j++)
        {
            remainPoint -= values[j];
        }

        for (int i = 0; i < APokers.Count; i++)
        {
            int value = 0;
            if (remainPoint >= 11 && remainPoint - 11 >= ((APokers.Count - 1) - i))
            {
                value = 11;
            }
            else
            {
                value = 1;
            }
            remainPoint -= value;

            for (int j = 0; j < values.Count; j++) {
                if (values[j] == 0) {
                    values[j] = value;
                    break;
                }
            }
        }

        return values;
    }

    public void cardHandleTypeHandle(List<IUser> list,bool isNpc, CardHandleType type) {
        ICardHandlePara handlePara = new CardHandleParaObject();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].isNpc() == isNpc)
            {
                handlePara.setUser(list[i]);
                handlePara.setAttackUser(list[i]);
                break;
            }
        }
        CardMgr.Instance.handle(handlePara, type);
    }

    public void reDealHandPoker(List<IUser> list, bool isNpc) {
        IUser user = null;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].isNpc() == isNpc)
            {
                user = list[i];
                break;
            }
        }
        if (user != null)
        {
            HandPokerMgr.Instance.clearHandPoker(user);
            for (int i = 0; i < 2; i++)
            {
                IPoker poker = PokerPileMgr.Instance.dealPoker();
                poker.setBack(i == 0 && isNpc);
                HandPokerMgr.Instance.addHandPoker(user, poker);
            
                int number = HandPokerMgr.Instance.getHandPokerPoint(user, true);
                GameMessage.Instance.addMsg(GameConst.DEALPOKER, user, poker, number);
            }
            EventDispatcher.Instance.emit(GameConst.CLEARHEADPOKER, user);
        }

    }
}
