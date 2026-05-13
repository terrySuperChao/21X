//下一次造成伤害的 %s% 转化为回血
public class LifeStealHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Life_Steal";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("LifeStealHandle=========>>");
        float addValue = this.getAddValue(para);
        para.getAttackUser().getExtraInfo().setLifeSteal(addValue);
    }
}
