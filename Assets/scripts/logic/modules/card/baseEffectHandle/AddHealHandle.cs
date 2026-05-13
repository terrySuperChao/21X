//回复 %s 点生命值
public class AddHealHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Heal";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("SubArmorHandle=========>>");
        float addValue = this.getAddValue(para);
        para.getAttackUser().addBlood(addValue);
        IUICommonPara attackPara = new UICommonParaObject(para.getAttackUser(), ValueType.blood, addValue, para.getAttackUser().getBlood());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
    }
}
