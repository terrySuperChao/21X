//≈∆∂—
using System.Collections.Generic;
using UnityEngine;

public class CardFlow : GameFlowObject
{
    override
    protected void _gameBegin(IGameBeginPara para)
    {
        if (CardMgr.Instance.getRound() % 2 == 1)
        {
            List<IUser> users = para.getUsers();
            for (int i = 0; i < users.Count; i++)
            {
                List<ICard> cards = CardMgr.Instance.getRandomCard(users[i]);
                if (cards.Count > 0)
                {
                    GameMessage.Instance.addMsg(GameConst.DEALCARD, users[i], cards);
                }
            }
        }
        CardMgr.Instance.addRound();
    }

    override
    protected void _handPokerAfter(IHandPokerAfterPara para)
    {
        cardHandleTypeHandle(para.getUsers(), false, CardHandleType.handPokerAfter);
        cardHandleTypeHandle(para.getUsers(), true, CardHandleType.handPokerAfter);
    }


    override
    protected void _dealPokerAfter(IDealPokerAfterPara para)
    {
        cardHandleTypeHandle(para.getUsers(), !para.getUser().isNpc(), CardHandleType.dealPokerAfter);
    }

    override
    protected bool _gameSettle(IGameSettlePara para) {
        int winIndex = para.getWinIndex();
        if (winIndex == -1)
        {
            //±¨≈∆
            cardHandleTypeHandle(para.getUsers(), false, CardHandleType.roundBust);
            cardHandleTypeHandle(para.getUsers(), true, CardHandleType.roundBust);
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
                IUIPokerPara pokerPara = new UIPokerPara(attackUser, poker, addValue, finalValue, roundResult.getAttributeMult(), para.isBackJock());
                GameMessage.Instance.addMsg(GameConst.ADDPOKERVALUE, pokerPara);

                handlePara.setPoker(poker);
                handlePara.setBaseValue(addValue);
                CardMgr.Instance.handle(handlePara, CardHandleType.roundAddValueBefore);
                CardMgr.Instance.handle(handlePara, CardHandleType.roundAddValue);
            }

            //±¨≈∆
            cardHandleTypeHandle(para.getUsers(), false, CardHandleType.roundBust);
            cardHandleTypeHandle(para.getUsers(), true, CardHandleType.roundBust);

            //π•ª˜Ω·À„
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
            CardMgr.Instance.handle(handlePara, CardHandleType.roundEnd);

            return defenseUser.getBlood() <= 0 || attackUser.getBlood() <= 0;
        }
    }

    

    public void cardHandleTypeHandle(List<IUser> list,bool isNpc, CardHandleType type) {
        ICardHandlePara handlePara = new CardHandleParaObject();
        IRoundResult roundResult = new RoundResultObject();
        handlePara.setRoundResult(roundResult);
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

    public void reDealHandPoker(List<IUser> list, bool isNpc,int suit) {
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
                int suitValue = RandomMgr.Instance.getRangeInt(0, 2) == 0 ? 0 : suit;
                IPoker poker = PokerPileMgr.Instance.dealSpecialPoker(suitValue);
                poker.setBack(i == 0 && isNpc);
                HandPokerMgr.Instance.addHandPoker(user, poker);
            
                int number = HandPokerMgr.Instance.getHandPokerPoint(user, true);
                GameMessage.Instance.addMsg(GameConst.DEALPOKER, user, poker, number);
            }
            EventDispatcher.Instance.emit(GameConst.CLEARHEADPOKER, user);
        }
    }
}
