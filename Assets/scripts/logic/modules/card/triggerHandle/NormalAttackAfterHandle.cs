using System;
using System.Reflection;
using System.Collections.Generic;

public class NormalAttackAfterHandle : TriggerHandleObject
{
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.attackAfter;
    }

    protected override bool _roundAttackHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Attack After Win Handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "受到攻击后触发";
        if (logic.IndexOf(str) != 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
