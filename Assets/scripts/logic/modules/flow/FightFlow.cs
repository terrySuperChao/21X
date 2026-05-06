//�ƶ�
using System.Collections.Generic;

public class FightFlow : GameFlowObject
{

    private float _settleAattck = 0;
    private float _settleDefense = 0;
    private bool _isBlackJack = false;

    override
    protected bool _gameSettle(IGameSettlePara para) {

        _settleAattck = 0;
        _settleDefense = 0;
        _isBlackJack = para.isBlackJack();

        int mult = para.isBlackJack() ? 2 : 1;
        int winIndex = para.getWinIndex();
        List<IUser> users = para.getUsers();
        for (int i = 0; i < users.Count; i++) {
            if (i == winIndex) {
                float blood = 0;
                float attack = 0;
                float defense = 0;
                float magic = 0;

                List<IPoker> pokers = HandPokerMgr.Instance.getHandPoker(users[i]);
                List<int> values = PokerPointMgr.Instance.getPokerValue(pokers);
                for (int j = 0; j < pokers.Count; j++) {
                    int value = values[j];
                    switch (pokers[j].getSuit()) {
                        case 1: // ��
                            defense += value * 0.5f;
                            break;
                        case 2: // ��
                            blood += value * 0.5f;
                            break;
                        case 3: // ��
                            attack += value;
                            break;
                        case 4: // ÷
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

    public float getSettleAttack() {
        return _settleAattck;
    }

    public float getSettleDefense()
    {
        return _settleDefense;
    }

    public bool isBackJack() {
        return _isBlackJack;
    }
}
