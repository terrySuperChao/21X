using System.Collections.Generic;

public class GameRunTimeMgr : Singleton<GameRunTimeMgr>
{
    //1005
    private readonly int _id1005 = GameCardConst.TriggerEffectId1005;
    //1012
    private readonly int _id1012 = GameCardConst.TriggerEffectId1012;
    //1022
    private readonly int _id1022 = GameCardConst.TriggerEffectId1022;

    //运行时:获得攻击力
    public void runTimeCountAttack(ITriggerHandlePara para, float addValue) {
        List<IAssembleCard> cards = ImprintDataMgr.Instance.getAssembleCard(para.getAttackUser().isNpc());
        int index = cards.FindIndex(card => card.getTriggerId() == this._id1005);
        if (index == -1) return;

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
        GameCardMgr.Instance.handle(para, TriggerEvent.CUSTOM_EVENT,this._id1005, baseEffectValue.getValue());        
    }

    //符合条件
    public void lessRunTimeCountAttack(ITriggerHandlePara para, float addValue) {
        IBaseEffectData data = para.getAttackUser().getExtraInfo().getBaseEffectData(this._id1005);
        IBaseEffectValue baseEffectValue = data.getBaseEffectValue(BaseEffectType.rtCountAttack);
        baseEffectValue.addValue(addValue);
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

    //受到伤害
    public void runTimeRoundGetHurt(IUser defenseUser)
    {
        IBaseEffectData data = defenseUser.getExtraInfo().getBaseEffectData(this._id1022);
        if (!data.isState())
        {
            data.setState(1);
        }
    }

    public bool getRunTimeRoundGetHurt(IUser defenseUser)
    {
        IBaseEffectData data = defenseUser.getExtraInfo().getBaseEffectData(this._id1022);
        return data.isState();
    }

    public void clearRunTimeRoundGetHurt(IUser defenseUser)
    {
        IBaseEffectData data = defenseUser.getExtraInfo().getBaseEffectData(this._id1022);
        if (!data.isState())
        {
            data.setState(1);
        }
    }
}
