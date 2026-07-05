public class GameRunTimeMgr : Singleton<GameRunTimeMgr>
{
    private readonly int _id = TriggerHandleMgr.TriggerEffectId1005;
    private readonly int _maxValue = 20;//每累计获得攻击力20点

    //运行时:获得攻击力
    public void runTimeCountAttack(ITriggerHandlePara para, float addValue) {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.rtCountAttack);
        if (!data.isState())
        {
            data.setState(1);
            baseEffectValue.setValue(addValue);
        }
        else
        {
            baseEffectValue.addValue(addValue);
        }

        //每累计获得攻击力20点
        if (baseEffectValue.getValue() >= this._maxValue)
        {
            CardMgr.Instance.handle(para, TriggerEvent.CUSTOM_EVENT);
        }
    }

    //符合条件
    public float getRunTimeCountAttack(ITriggerHandlePara para) {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.rtCountAttack);

        float value = baseEffectValue.getValue();
        if (baseEffectValue.getValue() >= this._maxValue) {
            baseEffectValue.addValue(-this._maxValue);
        }
        return value;
    }

    //符合条件
    public float getRunTimeCountAttack(IUser attackUser)
    {
        IBaseEffectData data = attackUser.getExtraInfo().getBaseEffectData(this._id);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.rtCountAttack);
        return baseEffectValue.getValue();
    }
}
