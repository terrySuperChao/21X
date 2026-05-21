//若下次普通攻击暴击，则保留50%的攻击力
public class OverHealATKHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Over_Heal_ATK";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("OverHealATKHandle=========>>");
        float addValue = 1.0f;
        para.getAttackUser().getExtraInfo().setOverHealATK(addValue);   
    }
}
