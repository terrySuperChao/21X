//∫⁄Ã“¥Û ¶
using UnityEngine;

public class SpadeCardPlusHandle : SpadeCardHandle
{
    override
    protected void _roundAttackBeforeHandle(ICardHandlePara para)
    {
        if (RandomMgr.Instance.getRangeInt(0, 2) == 0)
        {
            para.getRoundResult().setPenetrateValue(1);

            IUIFlyFontPara uiPara1 = new UIFlyFontParaObject(para.getAttackUser(), para.getCard(), "¥©Õ∏");
            GameMessage.Instance.addMsg(GameConst.FLYFONT, uiPara1);
        }
    }

    override
    public int getNumber()
    {
        return 2;
    }
}
