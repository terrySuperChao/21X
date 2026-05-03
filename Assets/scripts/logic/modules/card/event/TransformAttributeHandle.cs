using System.Collections.Generic;
public class TransformAttributeHandle : TriggerHandleObject
{
    private Dictionary<string, PokerSuit> _dic =  new Dictionary<string, PokerSuit>{
        {"方块", PokerSuit.diamond},
        {"红桃", PokerSuit.heart},
        {"黑桃", PokerSuit.spade},
        {"梅花", PokerSuit.club},
    };
    protected override TriggerEvent _getTrigger() {
        return TriggerEvent.transformAttribute;
    }

    protected override void _roundAddValueHandle(ITriggerHandlePara para)
    {
        UnityEngine.Debug.Log("transform attribute handle");
        string keystr = "";
        string logic = para.getAssembleCard().getTrigger().getLogic();
        foreach (var key in this._dic.Keys)
        {
            if (logic.IndexOf(key) > -1) {
                keystr = key;
                break;
            }
        }
        
        if (this._dic.ContainsKey(keystr) &&
            this._dic[keystr] == (PokerSuit)para.getPoker().getSuit())
        {
            UnityEngine.Debug.Log("transform attribute handle ======" + keystr);
            float addValue = 1;
            float finalValue = para.getAttackUser().addMagic(addValue);

            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getAssembleCard(), "+" + addValue);
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

            IUICommonPara uiPara2 = new UICommonParaObject(para.getAttackUser(), GameConst.SuitTransformValueType(this._dic[keystr]), addValue, finalValue);
            GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
        }
    }
}
