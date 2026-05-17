using System;
using System.Collections.Generic;
//敌方当前护甲 > 0
//敌方当前血量 < 40%
public class AttackBeforeLossHandle : TriggerHandleObject
{
    private Dictionary<string, Func<ITriggerHandlePara, float>> _dic = new Dictionary<string, Func<ITriggerHandlePara, float>>();
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.roundAttackBefore;
    }

    protected override bool _roundAttackBeforeHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Attack Before Loss Handle");

        if (this._dic.Count == 0) {
            this._dic.Add("敌方当前护甲", this.getDefenseFunc);
            this._dic.Add("敌方当前血量", this.getBloodFunc);
        }

        string keystr = "";
        string remainStr = "";
        string logic = para.getAssembleCard().getTrigger().getLogic();
        foreach (var key in this._dic.Keys)
        {
            if (logic.IndexOf(key) > -1)
            {
                keystr = key;
                remainStr = logic.Replace(key, "");
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

    private float getDefenseFunc(ITriggerHandlePara para) {
        return para.getDefenseUser().getDefense();
    }

    private float getBloodFunc(ITriggerHandlePara para) {
        return para.getDefenseUser().getBlood() / para.getDefenseUser().getMaxBlood() * 100.0f;
    }
}
