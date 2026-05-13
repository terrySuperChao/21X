//下次转化方块属性，额外获得 %s% 的护甲
public class BonusArmorHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Bonus_Armor";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("BonusArmorHandle=========>>");
        float addValue = this.getAddValue(para);
        para.getAttackUser().getExtraInfo().setBonusArmor(addValue);
    }
}
