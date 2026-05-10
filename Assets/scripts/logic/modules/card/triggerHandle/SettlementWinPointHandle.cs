using System.Collections.Generic;
//获胜且点数 >= 20
public class SettlementWinPointHandle : TriggerHandleObject
{
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.settlementBefore;
    }

    protected override bool _settlementBeforeHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Settlement Point handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "获胜且点数";
        if (logic.IndexOf(str) != 0) {
            return false;
        }

        float point = (float)FightPokerMgr.Instance.getUserHandPokerPoint(para.getAttackUser(), false);
        string compareStr = logic.Replace(str, "");
        return this.compareLogic(compareStr,point);
    }
}
