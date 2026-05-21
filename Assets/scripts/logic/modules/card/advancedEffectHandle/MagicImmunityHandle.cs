//获得 1个技能免疫的护盾，无法叠加
public class MagicImmunityHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Magic_Immunity";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("MagicImmunityHandle=========>>");
        float addValue = 1;
        para.getAttackUser().getExtraInfo().setMagicImmunity(addValue);   
    }
}
