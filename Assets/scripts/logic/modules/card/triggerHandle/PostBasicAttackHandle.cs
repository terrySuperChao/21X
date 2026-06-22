//受到攻击后触发
public class PostBasicAttackHandle : TriggerHandleObject
{
    protected override bool _postBasicAttackHandle(ITriggerHandlePara para)
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
                return !para.getAttackUser().isNpc();
            }
            else
            {
                return para.getAttackUser().isNpc();
            }
        }
    }
}
