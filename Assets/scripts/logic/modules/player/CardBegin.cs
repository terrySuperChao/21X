using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class CardBegin : IGameBegin
{
    public void gameBegin(IGameBeginPara para)
    {
        List<IUser> users = para.getUsers();
        for (int i = 0; i < users.Count; i++) {
            List<ICard> cards = CardMgr.Instance.getRandomCard(users[i]);
            GameCtrl.Instance.addMsg(GameConst.DEALCARD, users[i], cards);
        }
    }
}
