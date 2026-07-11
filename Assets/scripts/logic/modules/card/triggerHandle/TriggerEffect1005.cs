//每累计获得攻击力20点
public class TriggerEffect1005 : TriggerHandleObject
{
    private readonly int _id = GameCardConst.TriggerEffectId1005;
    private readonly int _max = 20;
    protected override int _getId()
    {
        return this._id;
    }

    protected override bool _customEventHandle(ITriggerHandlePara para)
    {
        if (para.getTemporaryValue() >= this._max)
        {
            GameRunTimeMgr.Instance.lessRunTimeCountAttack(para, -this._max);
            return true;
        }
        else {
            return false;
        }
    }
}
