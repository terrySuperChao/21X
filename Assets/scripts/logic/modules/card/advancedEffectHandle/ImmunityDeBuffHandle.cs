//下回合免疫负面状态的伤害
public class ImmunityDeBuffHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Immunity_DeBuff";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddBleedingHandle=========>>");
        float addValue = 1;
        para.getAttackUser().getExtraInfo().setImmunityDeBuff(addValue);   
    }
}
