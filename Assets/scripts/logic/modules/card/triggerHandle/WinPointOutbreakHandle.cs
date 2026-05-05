using System.Collections.Generic;
//胜点爆发
public class WinPointOutbreakHandle : TriggerHandleObject
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

    protected override bool _roundAddValueHandle(ITriggerHandlePara para)
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
            return true;
        }
        else {
            return false;
        }
    }
}
