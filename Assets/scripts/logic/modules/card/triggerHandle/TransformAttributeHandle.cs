using System.Collections.Generic;
//属性转换
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

    protected override bool _transformAttributeHandle(ITriggerHandlePara para)
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
            return true;
        }
        else {
            return false;
        }
    }
}
