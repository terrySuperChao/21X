using System;
using System.Collections.Generic;
using UnityEngine;

public class UserMgr : Singleton<UserMgr>
{
    private IUser _user = new UserObject(false);
    public void init() {
        _user.setMoney(ConfigMgr.INIT_MONEY_VALUE);
        _user.setBlood(ConfigMgr.INIT_BLOOD_VALUE);
        _user.setMaxBlood(ConfigMgr.INIT_BLOOD_VALUE);
        _user.setMaxMagic(ConfigMgr.INIT_MAGIC_VALUE);
    }

    public IUser getUser() {
        return _user;
    }
}
