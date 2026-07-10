public class GameRunTimeMgr : Singleton<GameRunTimeMgr>
{
    //1005
    private readonly int _id1005 = GameCardConst.TriggerEffectId1005;
    private readonly int _maxValue1005 = 20;//每累计获得攻击力20点

    //1012
    private readonly int _id1012 = GameCardConst.TriggerEffectId1012;

    //运行时:获得攻击力
    public void runTimeCountAttack(ITriggerHandlePara para, float addValue) {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id1005);
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
        if (baseEffectValue.getValue() >= this._maxValue1005)
        {
            GameCardMgr.Instance.handle(para, TriggerEvent.CUSTOM_EVENT,this._id1005);
        }
    }

    //符合条件
    public float getRunTimeCountAttack(ITriggerHandlePara para) {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id1005);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.rtCountAttack);

        float value = baseEffectValue.getValue();
        if (baseEffectValue.getValue() >= this._maxValue1005) {
            baseEffectValue.addValue(-this._maxValue1005);
        }
        return value;
    }

    //符合条件
    public float getRunTimeCountAttack(IUser attackUser)
    {
        IBaseEffectData data = attackUser.getExtraInfo().getBaseEffectData(this._id1005);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.rtCountAttack);
        return baseEffectValue.getValue();
    }

    //受到普通攻击后，有被护甲抵挡过
    public void runTimeConsumeDefense(IUser defenseUser)
    {
        if (defenseUser.getDefense() <= 0) {
            return;
        }

        IBaseEffectData data = defenseUser.getExtraInfo().getBaseEffectData(this._id1012);
        if (!data.isState()){
            data.setState(1);
        }
    }

    public bool getRunTimeConsumeDefense(IUser defenseUser)
    {
        IBaseEffectData data = defenseUser.getExtraInfo().getBaseEffectData(this._id1012);
        return data.isState();
    }

    public void clearRunTimeConsumeDefense(IUser defenseUser) {
        IBaseEffectData data = defenseUser.getExtraInfo().getBaseEffectData(this._id1012);
        data.setState(0);
    }
}
