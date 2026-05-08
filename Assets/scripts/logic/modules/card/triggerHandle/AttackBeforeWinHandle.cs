using System;
using System.Reflection;
using System.Collections.Generic;
//自身护甲值 >= 30
//当前 MP >= 50
public class AttackBeforeWinHandle : TriggerHandleObject
{
    private Dictionary<string, Func<ITriggerHandlePara,float>> _dic = new Dictionary<string, Func<ITriggerHandlePara,float>>();
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.attackBefore;
    }

    protected override bool _roundAttackBeforeHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Attack Before Loss Handle");

        if (this._dic.Count == 0) {
            this._dic.Add("自身护甲值", this.getDefenseFunc);
            this._dic.Add("当前 MP", this.getMagicFunc);
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

    private float getDefenseFunc(ITriggerHandlePara para) {
        return para.getUser().getDefense();
    }

    private float getMagicFunc(ITriggerHandlePara para) {
        return para.getUser().getMagic();
    }
}
