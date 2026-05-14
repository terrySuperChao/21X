//下次普通攻击使敌方获得 3 层流血状态
public class AddBleedingHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Add_Bleeding";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddBleedingHandle=========>>");
        float addValue = 3;
        para.getDefenseUser().getExtraInfo().setAddBleeding(addValue);   
    }
}
