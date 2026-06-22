public class PostMainSkillHandle : TriggerHandleObject
{
    protected override bool _postMainSkillHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("Attack After Win Handle");
        string logic = para.getAssembleCard().getTrigger().getLogic();
        string str = "释放主技能后";
        if (logic.IndexOf(str) != 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
