//受到攻击后触发
public class NormalAttackAfterHandle : TriggerHandleObject
{
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.normalAttackAfter;
    }

    protected override bool _normalAttackAfterHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Attack After Win Handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "受到攻击后触发";
        if (logic.IndexOf(str) != 0)
        {
            return false;
        }
        else
        {
            //0:npc 1:player
            if (para.getGameSettlePara().getWinIndex() == 0)
            {
                return !para.getUser().isNpc();
            }
            else
            {
                return para.getUser().isNpc();
            }
        }
    }
}
