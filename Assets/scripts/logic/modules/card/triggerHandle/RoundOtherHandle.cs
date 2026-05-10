using System;
using System.Collections.Generic;

public class RoundOtherHandle : TriggerHandleObject
{
    private Dictionary<string, Func<ITriggerHandlePara, float>> _dic = new Dictionary<string, Func<ITriggerHandlePara, float>>();
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.roundOther;
    }

    protected override bool _roundAttackBeforeHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Attack Before Loss Handle");

        if (this._dic.Count == 0) {
            this._dic.Add("单次造成伤害", this.getHurtValue);
            this._dic.Add("本轮获得法力值", this.getMagicValue);
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
        return this.compareLogic(compareStr,number);
    }

    private float getHurtValue(ITriggerHandlePara para) {
        return para.getRoundResult(para.getAttackUser()).getHurtVaule();
    }

    private float getMagicValue(ITriggerHandlePara para) {
        return para.getRoundResult(para.getAttackUser()).getMagicValue();
    }
}
