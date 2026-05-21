//下次治疗效果改为对对手造成等量伤害
public class HealToDMGHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Heal_to_DMG";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("HealToDMGHandle=========>>");
        float addValue = 1.0f;
        para.getAttackUser().getExtraInfo().setHealToDMG(addValue);   
    }
}
