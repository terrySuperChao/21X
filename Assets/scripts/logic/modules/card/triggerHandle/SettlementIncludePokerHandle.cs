using System.Collections.Generic;
//获胜且手牌包含 A
public class SettlementIncludePokerHandle : TriggerHandleObject
{
    private Dictionary<string, int> _dic = new Dictionary<string, int>{
        {"A", 14},
        {"K", 13},
        {"Q", 12},
        {"J", 11},
        {"10", 10},
        {"9", 9},
        {"8", 8},
        {"7", 7},
        {"6", 6},
        {"5", 5},
        {"4", 4},
        {"3", 3},
        {"2", 2},
    };

    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.settlementBefore;
    }

    protected override bool _roundAttackBeforeHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Settlement include poker handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "获胜且手牌包含";
        if (logic.IndexOf(str) != 0) {
            return false;
        }

        int rank = -1;
        string compareStr = logic.Replace(str, "");
        foreach (var key in this._dic.Keys)
        {
            if (compareStr.IndexOf(key) > -1)
            {
                rank = this._dic[key];
                break;
            }
        }
        
        if (rank == -1) {
            return false;
        }

        List<IPoker> pokers = FightPokerMgr.Instance.getUsetHandPoker(para.getUser());
        int index = pokers.FindIndex(poker => poker.getRank() == rank);
        return index != -1;
    }
}
