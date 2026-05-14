//下次普通攻击无视对手护甲
public class IgnoreArmorHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Ignore_Armor";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("IgnoreArmorHandle=========>>");
        float addValue = 1;
        para.getDefenseUser().getExtraInfo().setIgnoreArmor(addValue);   
    }
}
