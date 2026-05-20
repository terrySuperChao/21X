//若下次普通攻击暴击，则保留50%的攻击力
public class RetainATKHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Retain_ATK";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("RetainATKHandle=========>>");
        float addValue = 0.5f;
        para.getDefenseUser().getExtraInfo().setRetainATK(addValue);   
    }
}
