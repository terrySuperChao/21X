//受到普通攻击后，有被护甲抵挡过
public class TriggerEffect1012 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1012;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _postBasicAttackHandle(ITriggerHandlePara para)
    {
        if (GameRunTimeMgr.Instance.getRunTimeConsumeDefense(para.getDefenseUser()))
        {
            //0:npc 1:player
            if (para.getGameSettlePara().getWinIndex() == 0)
            {
                return !para.getDefenseUser().isNpc();
            }
            else
            {
                return para.getDefenseUser().isNpc();
            }
        }
        else
        {
            return false;
        }
    }
}
