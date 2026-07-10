//对手总点数 >= 22 (爆牌)
public class TriggerEffect1025 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1025;
    private readonly int _max = 22;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _postBattleResultHandle(ITriggerHandlePara para)
    {
        float point = (float)FightPokerMgr.Instance.getUserHandPokerPoint(para.getDefenseUser(), false);
        return point >= this._max;
    }
}
