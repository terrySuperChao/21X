//获得 3/5 点攻击力
public class AddATKHandle : BaseEffectHandleObject
{
    protected override string _getActionGenre() {
        return "Add_ATK";
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("AddATKHandle=========>>");
        float addValue = this.getAddValue(para);
        float finalValue = para.getAttackUser().addAttack(addValue);
      
        IUICommonPara uiPara = new UICommonParaObject(para.getAttackUser(), GameConst.SuitTransformValueType(PokerSuit.spade), addValue, finalValue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara);
    }
}
