//
using System.Collections.Generic;

public interface IBuffEffect
{ 
    public List<BaseEffectType> getBuffs(IUser user);
    public void addBuffType(IUser user, BaseEffectType type,bool addMsg = true);
    public void removeBuffType(IUser user, BaseEffectType type);
}
