//随机获得以下一种效果：攻击力+10或回复6点生命值或回复10点法力值或护甲+6
public class AdvancedEffect3901 : BaseEffectHandleObject
{
    private readonly int _attackValue = 10;
    private readonly int _bloodValue = 6;
    private readonly int _magicValue = 10;
    private readonly int _defenseValue = 6;
    private readonly int _id = AdvancedEffectHandleMgr.advancedEffectId3901;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        int index = RandomMgr.Instance.getRangeInt(0, 4);
        if (index == 0)
        {
            GameAttackMgr.Instance.handle(para, this._attackValue);
        }
        else if (index == 1)
        {
            GameBloodMgr.Instance.handle(para.getAttackUser(), this._bloodValue);
        }
        else if (index == 2)
        {
            GameMagicMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), this._magicValue);
        }
        else if (index == 3) {
            GameDefenseMgr.Instance.handle(para.getAttackUser(), para.getDefenseUser(), this._defenseValue);
        }
    }
}
