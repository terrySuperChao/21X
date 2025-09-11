//ÅÆ¶Ñ
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class GameSettlePara : IGameSettlePara
{
    private List<IUser> _users;
    private int _winIndex;
    private bool _isBackJock;
    public GameSettlePara(List<IUser> users, int winIndex, bool isBackJock) {
        _users = users;
        _winIndex = winIndex;
        _isBackJock = isBackJock;
    }
    public List<IUser> getUsers()
    {
        return _users;
    }

    public int getWinIndex()
    {
        return _winIndex;
    }

    public bool isBackJock()
    {
        return _isBackJock;
    }
}
