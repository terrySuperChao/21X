using System;
using System.Collections.Generic;

public class NpcMgr : Singleton<NpcMgr>
{
    private IUser _user = null;
    public void init() {
        _user = new UserObject(true);
        _user.setMoney(ConfigMgr.INIT_MONEY_VALUE);
        _user.setBlood(ConfigMgr.INIT_BLOOD_VALUE);
        _user.setMaxBlood(ConfigMgr.INIT_BLOOD_VALUE);
        _user.setMaxMagic(ConfigMgr.INIT_MAGIC_VALUE);
    }

    public IUser getUser() {
        return _user;
    }

}
