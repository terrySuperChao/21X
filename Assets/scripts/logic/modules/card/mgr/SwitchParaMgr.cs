using System;

public class SwitchParaMgr : Singleton<SwitchParaMgr>
{
    public void handle(ITriggerHandlePara para,Action callback,bool skip = false) {
        if (callback == null) {
            return;
        }

        IUser attackUser = para.getAttackUser();
        IUser defenseUser = para.getDefenseUser();

        if (!skip) {
            callback.Invoke(); // callback() 写法与callback.invoke()的区别是同步
        }
        para.setAttackUser(defenseUser);
        para.setDefenseUser(attackUser);

        callback.Invoke();
        para.setAttackUser(attackUser);
        para.setDefenseUser(defenseUser);
    }
}
