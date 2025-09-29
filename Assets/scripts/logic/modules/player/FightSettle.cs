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

    private float _settleAattck = 0;
    private float _settleDefense = 0;
    private bool _isBackJock = false;

    public bool gameSettle(IGameSettlePara para) {

        _settleAattck = 0;
        _settleDefense = 0;
        _isBackJock = para.isBackJock();

        int mult = para.isBackJock() ? 2 : 1;
        int winIndex = para.getWinIndex();
        List<IUser> users = para.getUsers();
        for (int i = 0; i < users.Count; i++) {
            if (i == winIndex) {
                float blood = 0;
                float attack = 0;
                float defense = 0;
                float magic = 0;

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
                users[i].addBlood(blood * mult);
                users[i].addAttack(attack * mult);
                users[i].addDefense(defense * mult);
                users[i].addMagic(magic * mult);
                break;
            }
        }

        for (int i = 0; i < users.Count; i++)
        {
            if (i != winIndex && winIndex > -1)
            {
                _settleAattck = users[winIndex].getAttack();
                _settleDefense = users[i].getDefense();
                float attack = _settleAattck;
                float defense = _settleDefense;
                float blood = users[i].getBlood();
                if (attack > defense) {
                    attack -= defense;
                    defense = 0;
                }
                else {
                    defense -= attack;
                    attack = 0;
                }

                if (blood > attack)
                {
                    blood -= attack;
                }
                else {
                    blood = 0;
                }
                users[i].setBlood(blood);
                users[i].setDefense(defense);
                users[winIndex].setAttack(0);
                break;
            }
        }

        bool isGameOver = false;
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].getBlood() <= 0)
            {
                isGameOver = true;
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


    public float getSettleAttack() {
        return _settleAattck;
    }

    public float getSettleDefense()
    {
        return _settleDefense;
    }

    public bool isBackJock() {
        return _isBackJock;
    }
}
