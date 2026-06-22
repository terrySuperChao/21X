using System.Collections.Generic;

public interface IBaseEffectData
{
    public void setId(int id);
    public int getId();
    public void setState(int state);
    public int getState();
    public bool isState();
    public void setBaseEffectValue(IBaseEffectValue value);
    public List<IBaseEffectValue> getBaseEffectValues();
    public IBaseEffectValue getBaseEffectValue(BaseEffectType type);
}
