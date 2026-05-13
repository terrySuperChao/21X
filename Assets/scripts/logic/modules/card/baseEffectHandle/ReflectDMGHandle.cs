//受到攻击时反弹 %s 点伤害
public class ReflectDMGHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Reflect_DMG";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("ReflectDMGHandle=========>>");
        float addValue = this.getAddValue(para);
        para.getAttackUser().getExtraInfo().setReflectDMG(addValue);
    }
}
