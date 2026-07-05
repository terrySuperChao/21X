using System.Collections.Generic;
//获胜且点数 >= 20
public class PostBattleResultHandle : TriggerHandleObject
{
    private Dictionary<string, int> _pokerDic = new Dictionary<string, int>{
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

    protected override bool _postBattleResultHandle(ITriggerHandlePara para)
    {
        return this.winHandle(para) ||
               this.lossHandle(para) ||
               this.drawHandle(para) ||
               this.winAndConainAHandle(para);
    }

    private bool winHandle(ITriggerHandlePara para) {
        UnityEngine.Debug.Log("Settlement Point handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "获胜且点数";
        if (logic.IndexOf(str) != 0)
        {
            return false;
        }

        //获胜的判断
        if (para.getGameSettlePara().getWinIndex() == -1)
        {
            return false;
        }

        if (para.getGameSettlePara().getWinIndex() == 0 && !para.getAttackUser().isNpc())
        {
            return false;
        }

        if (para.getGameSettlePara().getWinIndex() == 1 && para.getAttackUser().isNpc())
        {
            return false;
        }

        float point = (float)FightPokerMgr.Instance.getUserHandPokerPoint(para.getAttackUser(), false);
        string compareStr = logic.Replace(str, "");
        return this.compareLogic(compareStr, point);
    }

    private bool lossHandle(ITriggerHandlePara para) {
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "对手总点数";
        if (logic.IndexOf(str) != 0)
        {
            return false;
        }

        float point = (float)FightPokerMgr.Instance.getUserHandPokerPoint(para.getDefenseUser(), false);
        string compareStr = logic.Replace(str, "");
        return this.compareLogic(compareStr, point);
    }

    private bool drawHandle(ITriggerHandlePara para) {
        if (para.getGameSettlePara().getWinIndex() != -1)
        {
            return false;
        }

        UnityEngine.Debug.Log("Settlement Abortion handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "牌局平局";
        if (logic.IndexOf(str) != 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    protected bool winAndConainAHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Settlement include poker handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "获胜且手牌包含";
        if (logic.IndexOf(str) != 0)
        {
            return false;
        }

        //获胜的判断
        if (para.getGameSettlePara().getWinIndex() == -1)
        {
            return false;
        }

        if (para.getGameSettlePara().getWinIndex() == 0 && !para.getAttackUser().isNpc())
        {
            return false;
        }

        if (para.getGameSettlePara().getWinIndex() == 1 && para.getAttackUser().isNpc())
        {
            return false;
        }

        int rank = -1;
        string compareStr = logic.Replace(str, "");
        foreach (var key in this._pokerDic.Keys)
        {
            if (compareStr.IndexOf(key) > -1)
            {
                rank = this._pokerDic[key];
                break;
            }
        }

        if (rank == -1)
        {
            return false;
        }

        List<IPoker> pokers = FightPokerMgr.Instance.getUserHandPoker(para.getAttackUser());
        int index = pokers.FindIndex(poker => poker.getRank() == rank);
        return index != -1;
    }
}
