//方块大师+
using System.Collections.Generic;
using UnityEngine;

public class DiamondCardPlusHandle : DiamondCardHandle
{
    private Dictionary<string,float> _value = new Dictionary<string, float>();
    override
    protected void _roundSpecialAttrHandle(ICardHandlePara para) {
        if (para.getDefenseUser().getAttack() > para.getAttackUser().getDefense()){
            _value[para.getUser().getUserId()] = para.getDefenseUser().getAttack() * 0.5f;
        }
        else {
            _value[para.getUser().getUserId()] = para.getAttackUser().getDefense() * 0.5f;
        }
    }

    override
   protected void _roundSubDefenseHandle(ICardHandlePara para)
   {
        float value = _value[para.getUser().getUserId()];
        float addValue = -value;
        float finalValue = para.getDefenseUser().addBlood(addValue);
        IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "反弹伤害" + addValue);
        GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);

        IUICommonPara uiPara2 = new UICommonParaObject(para.getDefenseUser(), ValueType.blood, addValue, finalValue);
        GameMessage.Instance.addMsg(GameConst.ADDCARDVALUE, uiPara2);
   }

    override
    protected float getNumber()
    {
        return 0.5f;
    }
}
