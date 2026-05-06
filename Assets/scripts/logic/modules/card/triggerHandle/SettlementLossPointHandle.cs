using System.Collections.Generic;
//对手总点数 >= 22 (爆牌)
public class SettlementLossPointHandle : TriggerHandleObject
{
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.settlement;
    }

    protected override bool _roundBeginBeforeHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Settlement Point handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "对手总点数";
        if (logic.IndexOf(str) != 0)
        {
            return false;
        }

        float point = (float)FightPokerMgr.Instance.getUserHandPokerPoint(para.getDefenseUser(),false);
        string compareStr = logic.Replace(str, "");
        return this.compareLogic(compareStr,point);
    }
}
