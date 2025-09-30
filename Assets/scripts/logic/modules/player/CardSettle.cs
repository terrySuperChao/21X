//ÅÆ¶Ñ
using System.Collections.Generic;
using UnityEngine;

public class CardSettle : IGameSettle
{
    public bool gameSettle(IGameSettlePara para) {
        bool isPenetrate = false; //´©Í¸
        bool isGameOver = false;
        int winIndex = para.getWinIndex();
        List<IUser> users = para.getUsers();
        for (int i = 0; i < users.Count; i++) {
            if (i == winIndex) {
                IUser user = users[i];
                ICardHandlePara handlePara = new CardHandleParaObject(user, null,null,null);
                List<IPoker> pokers = HandPokerMgr.Instance.getHandPoker(user);
                List<int> values = getPokerValue(pokers);
                for (int j = 0; j < pokers.Count; j++) {
                    IPoker poker = pokers[j];
                    float addValue = values[j];
                    float finalValue = 0;
                    PokerSuit suit = (PokerSuit)poker.getSuit();
                    switch (suit) {
                        case PokerSuit.diamond: // ·½
                            addValue *= 0.5f;
                            finalValue = user.addDefense(addValue);
                            break;
                        case PokerSuit.heart: // ºì
                            addValue *= 0.5f;
                            finalValue = user.addBlood(addValue);
                            break;
                        case PokerSuit.spade: // ºÚ
                            addValue *= 1.0f;
                            finalValue = user.addAttack(addValue);
                            break;
                        case PokerSuit.club: // Ã·
                            addValue *= 1.0f;
                            finalValue = user.addMagic(addValue);
                            break;
                        default:
                            break;
                    }
                    IUIPokerPara pokerPara = new UIPokerPara(user, poker, addValue, finalValue, para.isBackJock());
                    GameMessage.Instance.addMsg(GameConst.ADDPOKERVALUE, pokerPara);

                    handlePara.setPoker(poker);
                    handlePara.setBaseValue(addValue);
                    CardMgr.Instance.handle(handlePara,CardHandleType.addValue);
                }

                CardMgr.Instance.handle(handlePara, CardHandleType.addRoundValue);
                if (handlePara.getExtralData() != null && (int)handlePara.getExtralData() == 1)
                {
                    isPenetrate = true;
                }
                break;
            }
        }

        for (int i = 0; i < users.Count; i++)
        {
            if (i != winIndex && winIndex > -1)
            {
                IUser user = users[winIndex];
                List<bool> skipDefense = new List<bool>() { isPenetrate };
                List<float> attacks = new List<float>() { user.getAttack() };

                if (user.getMagic() >= user.getMaxMagic())
                {
                    attacks.Add(50); //Ö±½Ó¹¥»÷50
                    skipDefense.Add(true);
                    user.setMagic(0);
                }

                for (int j = 0; j < attacks.Count; j++) {
                    if (attacks[j] <= 0){
                        continue;
                    }

                    float attack = attacks[j];
                    float defense = users[i].getDefense();
                    float blood = users[i].getBlood();
                    
                    float attackValue = attack;
                    float defenseValue = 0;
                    float bloodValue = 0;
                    
                    if (!skipDefense[j]) {
                        if (attack > defense)
                        {
                            defenseValue = defense;
                            attack -= defense;
                            defense = 0;
                        }
                        else
                        {
                            defense -= attack;
                            attack = 0;
                        }
                    }

                    if (blood > attack)
                    {
                        bloodValue = attack;
                        blood -= attack;
                    }
                    else
                    {
                        bloodValue = blood;
                        blood = 0;
                    }

                    users[i].setBlood(blood);
                    users[i].setDefense(defense);
                    user.setAttack(0);

                    IUICommonAttackPara attackPara = new UICommonAttackParaObject(user, users[i], attackValue, bloodValue, blood,defenseValue,defense,j == 1);
                    GameMessage.Instance.addMsg(GameConst.COMMONATTACK, attackPara);

                    if (blood <= 0) {
                        isGameOver = true;
                        break;
                    }   
                }
                break;
            }
        }
        return isGameOver;
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
}
