//牌堆
using System;
using System.Collections.Generic;

public class ConfigMgr: Singleton<UserMgr>
{
    //初始化钱的数量
    public const int INIT_MONEY_VALUE = 500;
    public const int INIT_BLOOD_VALUE = 100;
    public const int INIT_MAGIC_VALUE = 50;
}
