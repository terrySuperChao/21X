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

        //获胜的判断
        if (para.getGameSettlePara().getWinIndex() == -1) {
            return false;
        }

        if (para.getGameSettlePara().getWinIndex() == 0 && !para.getAttackUser().isNpc()) {
            return false;
        }

        if (para.getGameSettlePara().getWinIndex() == 1 && para.getAttackUser().isNpc()) {
            return false;
        }

        float point = (float)FightPokerMgr.Instance.getUserHandPokerPoint(para.getAttackUser(), false);
        string compareStr = logic.Replace(str, "");
        return this.compareLogic(compareStr,point);
    }
}
