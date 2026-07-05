using System;
using System.Collections.Generic;
//开始行动时，敌方当前护甲 > 0
//开始行动时，敌方当前血量 < 40%
public class PreActionHandle : TriggerHandleObject
{
    private Dictionary<string, Func<ITriggerHandlePara, float>> _dic = new Dictionary<string, Func<ITriggerHandlePara, float>>();

    protected override bool _preActionHandle(ITriggerHandlePara para)
    {
        if (this._dic.Count == 0) {
            this._dic.Add("开始行动时，敌方当前护甲", this.getDefenseFunc);
            this._dic.Add("开始行动时，敌方当前血量", this.getBloodFunc);
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
