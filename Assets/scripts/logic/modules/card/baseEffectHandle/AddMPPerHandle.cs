//额外获得当前法力值的 %s% 的法力值
public class AddMPPerHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Add_MP_Per";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddAMPHandle=========>>");
        float magic = para.getAttackUser().getMagic();
        float addValue = this.getAddValue(para) * magic;
        float finalValue = para.getAttackUser().addMagic(addValue);

        IUICommonPara attackPara = new UICommonParaObject(para.getAttackUser(), ValueType.magic, addValue, para.getAttackUser().getMagic());
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, attackPara);
    }
}
