using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Skill
{
    public string name;
    public string desc;
    public int unLockLevel;
}

[System.Serializable]
public class PlayerRole
{
    public int id;
    public string name;
    public string desc;
    public List<Skill> mainSkills;
    public List<Skill> secondSkills;
}

public class PlayerRoleConfig
{
    private readonly string _path = "config/playerRole";
    private List<PlayerRole> _playerRole = null;
    public void init()
    {
        this._playerRole = JsonMgr.Instance.readObject<List<PlayerRole>>(this._path);
    }

    public List<PlayerRole> getPlayerRole() {
        return this._playerRole;
    }
}
