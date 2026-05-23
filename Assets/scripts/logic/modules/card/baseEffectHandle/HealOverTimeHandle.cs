//接下来的2回合每回合回复 %s 点生命值
public class HealOverTimeHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Heal_Over_Time";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("HealOverTimeHandle=========>>");
        float addValue = this.getAddValue(para);
        //添加两次
        for (int i = 0; i < 2; i++){
            para.getAttackUser().getExtraInfo().setHealOverTime(addValue);
        }
    }
}
