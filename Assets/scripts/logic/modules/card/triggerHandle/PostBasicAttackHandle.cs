public class PostBasicAttackHandle : TriggerHandleObject
{
  
    protected override bool _postBasicAttackHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("PostBasicAttackHandle");
        return this.triggerId1012Func(para);
    }

    private bool triggerId1012Func(ITriggerHandlePara para) {
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "受到普通攻击后，有被护甲抵挡过";
        if (logic.IndexOf(str) != 0)
        {
            return false;
        }
        else
        {
            if (GameRunTimeMgr.Instance.getRunTimeConsumeDefense(para.getDefenseUser()))
            {
                //0:npc 1:player
                if (para.getGameSettlePara().getWinIndex() == 0)
                {
                    return !para.getDefenseUser().isNpc();
                }
                else
                {
                    return para.getDefenseUser().isNpc();
                }
            }
            else {
                return false;
            }  
        }
    }
}
