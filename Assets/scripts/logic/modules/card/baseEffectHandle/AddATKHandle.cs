using System.Collections.Generic;
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

        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getAssembleCard(), "+" + addValue);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

        PokerSuit suit = (PokerSuit)para.getPoker().getSuit();
        IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), GameConst.SuitTransformValueType(suit), addValue, finalValue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
    }
}
