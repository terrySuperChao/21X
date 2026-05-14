//下次普通攻击连续触发两次
public class DoubleProcHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Double_Proc";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddBleedingHandle=========>>");
        float addValue = 1;
        para.getDefenseUser().getExtraInfo().setDoubleProc(addValue);   
    }
}
