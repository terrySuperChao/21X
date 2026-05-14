//下次转化红桃属性，治疗量的 %s% 额外转化为法力值
public class HealToMPHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Heal_to_MP";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("HealToMPHandle=========>>");
        float addValue = this.getAddValue(para);
        para.getAttackUser().getExtraInfo().setHealToMP(addValue);
    }
}
