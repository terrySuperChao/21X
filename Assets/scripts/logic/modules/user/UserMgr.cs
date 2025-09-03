using System;
using System.Collections.Generic;

public class UserMgr : Singleton<UserMgr>
{
    private IUser _user = null;
    public void init() {
        _user = new UserObject(false);
        _user.setMoney(ConfigMgr.INIT_MONEY_VALUE);
    }

    public IUser getUser() {
        return _user;
    }

}
