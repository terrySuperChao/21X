using System.Collections.Generic;
//要牌 >= 3
public class DealPokerAfterHandle : TriggerHandleObject
{
    private Dictionary<string, int> _dic = new Dictionary<string, int>();
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.dealPokerAfter;
    }

    protected override bool _dealPokerAfterHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Deal Poker After Handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "要牌";
        if (logic.IndexOf(str) != 0)
        {
            return false;
        }

        string userId = para.getAttackUser().getUserId();
        if (!this._dic.ContainsKey(userId))
        {
            this._dic.Add(userId, 1);
        }
        else {
            this._dic[userId]++;
        }

        string compareStr = logic.Replace(str,"");
        return this.compareLogic(compareStr, this._dic[userId]);
    }

    protected override bool _settlementBeforeHandle(ITriggerHandlePara para)
    {
        this._dic.Clear();
        UnityEngine.Debug.Log("FFFFFFFFFFFFFF");
        return base._settlementBeforeHandle(para); 
    }
}
