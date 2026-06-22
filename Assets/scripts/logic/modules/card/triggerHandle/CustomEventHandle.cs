using System;
using System.Collections.Generic;

public class CustomEventHandle : TriggerHandleObject
{
    private Dictionary<string, Func<ITriggerHandlePara, float>> _dic = new Dictionary<string, Func<ITriggerHandlePara, float>>();
    protected override bool _turnStartHandle(ITriggerHandlePara para)
    {
        CardMgr.Instance.clearBaseEffectValue(para.getAttackUser(), BaseEffectType.rtMagicTotal);
        return base._turnStartHandle(para);
    }

    protected override bool _customEventHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Round Other Handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "单次造成伤害";
        if (logic.IndexOf(str) != 0){
            return false;
        }
        float number = para.getAttackUser().getExtraInfo().getRtHurtVaule();
        para.getAttackUser().getExtraInfo().clearRtHurtValue();//

        string compareStr = logic.Replace(str, "");
        return this.compareLogic(compareStr,number);
    }

    protected override bool _turnEndHandle(ITriggerHandlePara para) {
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "本轮获得法力值";
        if (logic.IndexOf(str) != 0){
            return false;
        }
        float number = CardMgr.Instance.getBaseEffectValue(para.getAttackUser(), BaseEffectType.rtMagicTotal);
        string compareStr = logic.Replace(str, "");
        return this.compareLogic(compareStr, number);
    }
}
