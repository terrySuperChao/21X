//下次造成伤害的 %s% 转化为回血，最多叠加5层
public class BaseEffect2022 : BaseEffectHandleObject
{
    private readonly int _initValue = 1;
    private readonly int _stepValue = 1;
    private readonly int _maxValue = 5;
    private readonly int _id = BaseEffectHandleMgr.baseEffectId2022;
    protected override int _getId()
    {
        return this._id;
    }

    protected override void _handle(ITriggerHandlePara para)
    {
        float addValue = this.getAddValue(para);
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue1 = data.getBaseEffectValue(BaseEffectType.addLevel);
        IBaseEffectValue baseEffectValue2 = data.getBaseEffectValue(BaseEffectType.lifeSteal);
        if (!data.isState())
        {
            data.setState(1);
            baseEffectValue1.setValue(this._initValue);
            baseEffectValue1.setMaxValue(this._maxValue);
            baseEffectValue2.setValue(addValue);
        }
        else
        {
            if (baseEffectValue1.getValue() < baseEffectValue1.getMaxValue())
            {
                baseEffectValue1.addValue(this._stepValue);
                baseEffectValue2.addValue(addValue);
            }
        }
    }

    protected override void _effect(IBaseEffectHandlePara para)
    {
        if (para.getEffectType() != AdvancedEffectType.enemyLessBlood) {
            return;
        }

        //使用真实血量
        float lifeSteal = CardMgr.Instance.getBaseEffectValue(para.getAttackUser(), BaseEffectType.lifeSteal);
        float addValue = lifeSteal * para.getExtralValue();
        GameBloodMgr.Instance.handle(para.getAttackUser(), addValue);
    }
}
