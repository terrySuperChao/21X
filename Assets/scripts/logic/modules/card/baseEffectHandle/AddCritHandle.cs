//固定增加 %s% 暴击率
public class AddCritHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Add_Crit";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddCritHandle=========>>");
        float addValue = this.getAddValue(para);
        para.getAttackUser().getExtraInfo().setAddCrit(addValue);
    }
}
