using System.Collections.Generic;
//平局
public class SettlementAbortionHandle : TriggerHandleObject
{
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.settlement;
    }

    protected override bool _roundAbortionHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Settlement Point handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "牌局平局";
        if (logic.IndexOf(str) != 0)
        {
            return false;
        }
        else {
            return true;
        }
    }
}
