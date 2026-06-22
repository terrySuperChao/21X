using System.Collections.Generic;
//要牌 >= 3
public class PostCardDrawHandle : TriggerHandleObject
{
    private Dictionary<string, int> _dic = new Dictionary<string, int>();
    protected override bool _postCardDrawHandle(ITriggerHandlePara para)
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
    
    protected override bool _preActionHandle(ITriggerHandlePara para)
    {
        this._dic.Clear();
        return base._preActionHandle(para); 
    }
}
