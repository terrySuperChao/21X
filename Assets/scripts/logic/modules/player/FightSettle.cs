//ÅÆ¶Ñ
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class FightSettle : IGameSettle
{

    public void gameSettle(IGameSettlePara para) {
        float blood = 0;
        float attack = 0;
        float defense = 0;
        float magic = 0;

        int winIndex = para.getWinIndex();
        List<IUser> users = para.getUsers();
        for (int i = 0; i < users.Count; i++) {
            if (i == winIndex) {
                List<IPoker> pokers = HandPokerMgr.Instance.getHandPoker(users[i]);
                List<int> values = getPokerValue(pokers);
                for (int j = 0; j < pokers.Count; j++) {
                    int value = values[j];
                    switch (pokers[j].getSuit()) {
                        case 1: // ·½
                            defense += value * 0.5f;
                            break;
                        case 2: // ºì
                            blood += value * 0.5f;
                            break;
                        case 3: // ºÚ
                            attack += value;
                            break;
                        case 4: // Ã·
                            magic += value;
                            break;
                        default:
                            break;
                    }
                }
                users[i].addBlood(blood);
                users[i].addAttcak(attack);
                users[i].addDefense(defense);
                users[i].addMagic(magic);
                break;
            }
        }

        for (int i = 0; i < users.Count; i++)
        {
            if (i != winIndex)
            {
                if (attack > users[i].getDefense()) {
                    attack -= users[i].getDefense();
                }
                else {
                    attack = 0;
                }
                users[i].addBlood(-attack);
                users[i].addDefense(-users[i].getDefense());
            }
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

}
