//下次技能效果提升 %s%，可叠加
public class SkillDamageUpHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Skill_Damage_Up";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("SkillDamageUpHandle=========>>");
        float addValue = this.getAddValue(para);
        para.getAttackUser().getExtraInfo().setSkillDamageUp(addValue);
    }
}
