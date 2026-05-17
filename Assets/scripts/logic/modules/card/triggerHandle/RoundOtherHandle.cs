using System;
using System.Collections.Generic;

public class RoundOtherHandle : TriggerHandleObject
{
    private Dictionary<string, Func<ITriggerHandlePara, float>> _dic = new Dictionary<string, Func<ITriggerHandlePara, float>>();
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.roundOther;
    }

    protected override bool _roundOtherHandle(ITriggerHandlePara para)
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

    protected override bool _roundAttackAfterHandle(ITriggerHandlePara para) {
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "本轮获得法力值";
        if (logic.IndexOf(str) != 0){
            return false;
        }
        float number = para.getAttackUser().getExtraInfo().getRtMagicValue();
        string compareStr = logic.Replace(str, "");
        return this.compareLogic(compareStr, number);
    }
}
