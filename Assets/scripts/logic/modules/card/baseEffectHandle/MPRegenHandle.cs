//接下来的3回合每回合回复 %s 点法力
public class MPRegenHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "MP_Regen";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("MPRegenHandle=========>>");
        float addValue = this.getAddValue(para);
        //添加3次
        for (int i = 0; i < 3; i++) {
            para.getAttackUser().getExtraInfo().setMpRegen(addValue);
        }
    }
}
