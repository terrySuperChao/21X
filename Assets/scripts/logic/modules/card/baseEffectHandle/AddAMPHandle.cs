//获得 %s 点法力值
public class AddAMPHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Add_MP";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddAMPHandle=========>>");
        float addValue = this.getAddValue(para);
        float finalValue = para.getAttackUser().addMagic(addValue);

        IUICommonPara attackPara = new UICommonParaObject(para.getAttackUser(), ValueType.magic, addValue, para.getAttackUser().getMagic());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
    }
}
