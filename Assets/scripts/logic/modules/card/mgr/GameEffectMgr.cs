using System;
using System.Collections.Generic;

public class GameEffectMgr : Singleton<GameEffectMgr>
{
    public float getBaseEffectValue(IUser user,BaseEffectType type) {
        float value = 0;
        List<IAssembleCard> cards = ImprintDataMgr.Instance.getAssembleCard(user.isNpc());
        for (int i = 0; i < cards.Count; i++) {
            IBaseEffectData data = user.getExtraInfo().getBaseEffectData(cards[i].getBaseEffectId());
            if (!data.isState()) continue;

            IBaseEffectValue baseEffectValue = data.getBaseEffectValues().Find(value=> value.getType() == type);
            if (baseEffectValue != null) {
                value += baseEffectValue.getValue();
            }
        }
        return value;
    }

    public float clearBaseEffectValue(IUser user, BaseEffectType type)
    {
        //移除
        FightPokerMgr.Instance.getBuffEffect().removeBuffType(user, type);

        float value = 0;
        List<IAssembleCard> cards = ImprintDataMgr.Instance.getAssembleCard(user.isNpc());
        for (int i = 0; i < cards.Count; i++)
        {
            IBaseEffectData data = user.getExtraInfo().getBaseEffectData(cards[i].getBaseEffectId());
            if (!data.isState()) continue;

            IBaseEffectValue baseEffectValue = data.getBaseEffectValues().Find(value => value.getType() == type);
            if (baseEffectValue != null) {
                data.setState(0);
                baseEffectValue.clearValue();
            }
        }
        return value;
    }

    public float subtractBaseEffectValue(IUser user, BaseEffectType type,float value) {
        List<IAssembleCard> cards = ImprintDataMgr.Instance.getAssembleCard(user.isNpc());
        for (int i = 0; i < cards.Count; i++)
        {
            IBaseEffectData data = user.getExtraInfo().getBaseEffectData(cards[i].getBaseEffectId());
            if (!data.isState()) continue;

            IBaseEffectValue baseEffectValue = data.getBaseEffectValues().Find(value => value.getType() == type);
            if (baseEffectValue == null){
                continue;
            }
            
            if (baseEffectValue.getValue() < value)
            {
                data.setState(0);
                value -= baseEffectValue.getValue();
                baseEffectValue.clearValue();
            }
            else {
                value = 0;
                baseEffectValue.addValue(-baseEffectValue.getValue());
            }
        }
        return value;
    }
}
