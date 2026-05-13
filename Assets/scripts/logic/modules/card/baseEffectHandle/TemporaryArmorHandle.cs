//获得当前护甲 %s% 的临时护甲
public class TemporaryArmorHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Temporary_Armor";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("SubArmorHandle=========>>");
        float addValue = this.getAddValue(para);
        float defense = para.getAttackUser().getDefense();
        addValue *= defense;
        para.getAttackUser().getExtraInfo().setTemporaryArmor(addValue);
        para.getAttackUser().setDefense(addValue);

        IUICommonPara attackPara = new UICommonParaObject(para.getAttackUser(), ValueType.defense, addValue, para.getAttackUser().getDefense());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
    }
}
