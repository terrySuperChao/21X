//将下次对手普通攻击的 50% 反弹给对手
public class ReflectPercentHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Reflect_Percent";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("RetainATKHandle=========>>");
        float addValue = 0.5f;
        para.getDefenseUser().getExtraInfo().setReflectPercent(addValue);   
    }
}
