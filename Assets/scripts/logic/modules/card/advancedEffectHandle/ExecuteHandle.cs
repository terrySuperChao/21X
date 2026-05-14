//下次普通攻击时若对手血量低于 15% 直接处决
public class ExecuteHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Execute";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("ExecuteHandle=========>>");
        float addValue = 15;
        para.getDefenseUser().getExtraInfo().setExecute(addValue);   
    }
}
