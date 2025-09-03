using System;
using System.Collections.Generic;

public class NpcMgr : Singleton<NpcMgr>
{
    private IUser _user = null;
    public void init() {
        _user = new UserObject(true);
        _user.setMoney(ConfigMgr.INIT_MONEY_VALUE);
    }

    public IUser getUser() {
        return _user;
    }

}
