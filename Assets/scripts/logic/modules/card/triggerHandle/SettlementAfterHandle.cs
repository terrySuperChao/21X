using System;
using System.Collections.Generic;
//
public class SettlementAfterHandle : TriggerHandleObject
{
    private Dictionary<string, Func<ITriggerHandlePara, float>> _dic = new Dictionary<string, Func<ITriggerHandlePara, float>>();
    protected override TriggerEvent _getTrigger()
    {
        return TriggerEvent.roundAttackAfter;
    }

    protected override bool _roundAttackAfterHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Settlement After Handle");

        if (this._dic.Count == 0)
        {
            this._dic.Add("自身血量", this.getBloodFunc);
        }

        string keystr = "";
        string remainStr = "";
        string logic = para.getAssembleCard().getTrigger().getLogic();
        foreach (var key in this._dic.Keys)
        {
            string str = key;
            if (logic.IndexOf(str) > -1)
            {
                keystr = key;
                remainStr = logic.Replace(str, "");
                break;
            }
        }

        if (!this._dic.ContainsKey(keystr))
        {
            return false;
        }

        float number = this._dic[keystr](para);
        string compareStr = remainStr;
        return this.compareLogic(compareStr, number);
    }

    private float getBloodFunc(ITriggerHandlePara para)
    {
        return para.getUser().getBlood() / para.getUser().getMaxBlood() * 100.0f;
    }
}
