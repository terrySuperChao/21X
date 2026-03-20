//≈∆∂—
using System.Collections.Generic;

public class CardFlow : GameFlowObject
{



    override
    protected void _gameBegin(IGameBeginPara para)
    {
        /*
        if (FightPokerMgr.Instance.addUserRound() % 2 == 0) 
            return;

        List<IUser> users = para.getUsers();
        foreach (var user in users){
            List<ICard> cards = CardMgr.Instance.getRandomCard(user);
            if (cards.Count > 0) {
                if (user.isNpc())
                {
                    ICard card = FightPokerMgr.Instance.addNpcCard(user, cards);
                    GameMessage.Instance.addMsg(GameConst.DEALCARD, new DealCardPara(user, card));
                    CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), true, CardHandleType.addNewCardAfter);
                }
                else {
                    GameMessage.Instance.addMsg(GameConst.CANDIDACYCARD, new CandidacyCardPara(user, cards));
                }
            }
        }*/
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), false, CardHandleType.addNewCardAfter);
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), true, CardHandleType.addNewCardAfter);
    }

    override
    protected void _addCardAfter(IAddCardAfterPara para)
    {
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), false, CardHandleType.addNewCardAfter);
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), true, CardHandleType.addNewCardAfter);
    }

    override
    protected void _handPokerAfter(IHandPokerAfterPara para)
    {
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), false, CardHandleType.handPokerAfter);
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), true, CardHandleType.handPokerAfter);
    }


    override
    protected void _dealPokerAfter(IDealPokerAfterPara para)
    {
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), !para.getUser().isNpc(), CardHandleType.dealPokerAfter);
    }

    override
    protected bool _gameSettle(IGameSettlePara para) {
        int winIndex = para.getWinIndex();
        if (winIndex == -1) //∆Ωæ÷
        {
            //±¨≈∆
            CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), false, CardHandleType.roundSpecialAttr);
            CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), true, CardHandleType.roundSpecialAttr);
            return false;
        }
        
        List<IUser> users = para.getUsers();
        IUser attackUser = users[winIndex];
        IUser defenseUser = users[winIndex == 0 ? 1 : 0];

        IRoundResult roundResult = new RoundResultObject();
        ICardHandlePara handlePara = new CardHandleParaObject();
        handlePara.setUser(attackUser);
        handlePara.setAttackUser(attackUser);
        handlePara.setDefenseUser(defenseUser);
        handlePara.setRoundResult(roundResult);

        CardMgr.Instance.handle( handlePara, CardHandleType.roundBegin);
        
        //ÃÌº”÷µ
        List<IPoker> pokers = FightPokerMgr.Instance.getUsetHandPoker(attackUser);
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
            IUIPokerPara pokerPara = new UIPokerPara(attackUser, poker, addValue, finalValue, roundResult.getAttributeMult(), para.isBackJock());
            GameMessage.Instance.addMsg(GameConst.ADDPOKERVALUE, pokerPara);

            handlePara.setPoker(poker);
            handlePara.setBaseValue(addValue);
            CardMgr.Instance.handle( handlePara, CardHandleType.roundAddValueBefore);
            CardMgr.Instance.handle( handlePara, CardHandleType.roundAddValue);
        }

        //±¨≈∆
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), attackUser.isNpc(), CardHandleType.roundSpecialAttr);
        CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), defenseUser.isNpc(), CardHandleType.roundSpecialAttr);

        //π•ª˜Ω·À„
        CardMgr.Instance.handle( handlePara, CardHandleType.roundAttackBegin);

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

                    CardMgr.Instance.cardHandleTypeHandle(para.getUsers(), defenseUser.isNpc(), CardHandleType.roundSubDefense);
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
            CardMgr.Instance.handle( handlePara, CardHandleType.roundSubBlood);
        }
        CardMgr.Instance.handle( handlePara, CardHandleType.roundAttackAfter);
        CardMgr.Instance.handle( handlePara, CardHandleType.roundEnd);

        return defenseUser.getBlood() <= 0 || attackUser.getBlood() <= 0;
        
    }
}
