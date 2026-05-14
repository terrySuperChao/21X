//回复 %s 点生命值，一场战斗仅生效一次
public class HealSuperHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Heal_Super";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("HealSuperHandle=========>>");
        float addValue = this.getAddValue(para);
        if (para.getAttackUser().getExtraInfo().getHealSuper() < 1) {
            para.getAttackUser().getExtraInfo().setHealSuper(1);
            GameBloodMgr.Instance.addBloodHandle(para.getAttackUser(), addValue);
        }   
    }
}
