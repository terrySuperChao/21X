using System;
using System.Collections.Generic;

public class CustomEventHandle : TriggerHandleObject
{
    private Dictionary<string, Func<ITriggerHandlePara, float>> _dic = new Dictionary<string, Func<ITriggerHandlePara, float>>();
    protected override bool _turnStartHandle(ITriggerHandlePara para)
    {
        GameCardMgr.Instance.clearBaseEffectValue(para.getAttackUser(), BaseEffectType.rtMagicTotal);
        return base._turnStartHandle(para);
    }

    protected override bool _customEventHandle(ITriggerHandlePara para)
    {
        if (this._dic.Count == 0)
        {
            this._dic.Add("单次造成伤害", this.getSingleHurtFunc);
            this._dic.Add("每累计获得攻击力", this.getCountAttackFunc);
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

    private float getSingleHurtFunc(ITriggerHandlePara para) {
        float number = para.getAttackUser().getExtraInfo().getRtHurtVaule();
        para.getAttackUser().getExtraInfo().clearRtHurtValue();//
        return number;
    }

    private float getCountAttackFunc(ITriggerHandlePara para) {
        return GameRunTimeMgr.Instance.getRunTimeCountAttack(para);
    }

    protected override bool _turnEndHandle(ITriggerHandlePara para) {
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "本轮获得法力值";
        if (logic.IndexOf(str) != 0){
            return false;
        }
        float number = GameCardMgr.Instance.getBaseEffectValue(para.getAttackUser(), BaseEffectType.rtMagicTotal);
        string compareStr = logic.Replace(str, "");
        return this.compareLogic(compareStr, number);
    }
}
