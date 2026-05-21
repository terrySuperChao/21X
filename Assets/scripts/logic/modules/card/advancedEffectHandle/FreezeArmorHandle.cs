//下次消耗的护甲会完全恢复
public class FreezeArmorHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Freeze_Armor";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("FreezeArmor=========>>");
        float addValue = 1.0f;
        para.getAttackUser().getExtraInfo().setFreezeArmor(addValue);   
    }
}
