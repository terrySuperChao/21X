//ÅÆ¶Ñ
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class GameBeginPara : IGameBeginPara
{
    private List<IUser> _users;
    public GameBeginPara(List<IUser> users) {
        _users = users;
    }
    public List<IUser> getUsers()
    {
        return _users;
    }
}
