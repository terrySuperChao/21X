//ÅÆ¶Ñ
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class CommonSettle : IGameSettle
{
    private int _money = 100;
    private int _settleMoney = 0;
    public void gameSettle(IGameSettlePara para)
    {
        int winIndex = para.getWinIndex();
        List<IUser> users = para.getUsers();

        int extraMoney = 0;
        if (para.isBackJock())
        {
            int number = 0;
            for (int i = 0; i < users.Count; i++)
            {
                if (winIndex != i)
                {
                    number = users[i].getMoney();
                    break;
                }
            }
            if (number >= _money / 2)
            {
                extraMoney = _money / 2;
            }
        }

        for (int i = 0; i < users.Count; i++)
        {
            if (winIndex == -1)
            {
                users[i].addMoney(_money);
            }
            else
            {
                if (winIndex == i)
                {
                    users[i].addMoney(_money * 2 + extraMoney);
                }
                else
                {
                    users[i].addMoney(-extraMoney);
                }
            }
        }

        _settleMoney = _money + extraMoney;
    }

    public int bettingMoney(List<IUser> users)
    {
        for (int i = 0; i < users.Count; i++)
        {
            _money = Math.Min(users[i].getMoney(), _money);
        }

        for (int i = 0; i < users.Count; i++)
        {
            users[i].addMoney(-_money);
        }
        return _money;
    }

    public int getSettleMoney() {
        return _settleMoney;
    }
}
