using System;
using System.Reflection;
using System.Collections.Generic;

public class MagicAttackAfterHandle : TriggerHandleObject
{
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.magicAttackAfter;
    }

    protected override bool _magicAttackAfterHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Attack After Win Handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "释放主技能后";
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
