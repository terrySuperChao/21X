using System;
using System.Collections.Generic;
//
public class TurnEndHandle : TriggerHandleObject
{
    private Dictionary<string, Func<ITriggerHandlePara, float>> _dic = new Dictionary<string, Func<ITriggerHandlePara, float>>();

    protected override bool _battleStartHandle(ITriggerHandlePara para)
    {
        if (this._dic.Count == 0)
        {
            this._dic.Add("自身血量", this.getBloodFunc);
            this._dic.Add("自身护甲值", this.getDefenseFunc);
            this._dic.Add("当前 MP", this.getMagicFunc);
        }
        return base._battleStartHandle(para);
    }


    protected override bool _turnEndHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Settlement After Handle");
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
        return para.getAttackUser().getBlood() / para.getAttackUser().getMaxBlood() * 100.0f;
    }

    private float getDefenseFunc(ITriggerHandlePara para)
    {
        return para.getAttackUser().getDefense();
    }

    private float getMagicFunc(ITriggerHandlePara para)
    {
        return para.getAttackUser().getMagic();
    }
}
