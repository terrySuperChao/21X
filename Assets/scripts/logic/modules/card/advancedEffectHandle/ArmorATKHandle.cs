//每次获得护甲对对手造成 5 点伤害
public class ArmorATKHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Armor_ATK";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("RetainATKHandle=========>>");
        float addValue = 5.0f;
        para.getAttackUser().getExtraInfo().setArmorATK(addValue);   
    }
}
