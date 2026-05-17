//获得 %s 点护甲
public class AddArmorHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Add_Armor";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddArmorHandle=========>>");
        float addValue = this.getAddValue(para);      
        para.getAttackUser().addDefense(addValue);

        IUICommonPara attackPara = new UICommonParaObject(para.getAttackUser(), ValueType.defense, addValue, para.getAttackUser().getDefense());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);

        float armorATK = para.getAttackUser().getExtraInfo().getArmorATK();
        if (armorATK > 0) {
            GameBloodMgr.Instance.lessBloodHandle(para.getAttackUser(), para.getDefenseUser(), armorATK);
        }
    }
}
