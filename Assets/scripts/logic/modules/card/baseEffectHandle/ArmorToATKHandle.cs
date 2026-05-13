//获得当前护甲 %s% 的攻击力
public class ArmorToATKHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Armor_to_ATK";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("ArmorToATKHandle=========>>");
        float addValue = this.getAddValue(para);
        
        float attck = para.getAttackUser().getDefense() * addValue;
        para.getAttackUser().addAttack(attck);

        IUICommonPara attackPara = new UICommonParaObject(para.getAttackUser(), ValueType.attack, addValue, para.getAttackUser().getAttack());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
    }
}
