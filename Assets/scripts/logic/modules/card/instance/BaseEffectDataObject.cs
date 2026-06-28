using System;
using System.Collections.Generic;

public class BaseEffectDataObject : IBaseEffectData
{
    private int _id = 0;
    private int _state = 0;
    private Func<BaseEffectType, IBaseEffectValue> _baseEffectValueInstance = null;
    private List<IBaseEffectValue> _baseEffectValues = new List<IBaseEffectValue>();
    
    public BaseEffectDataObject(int id) {
        this._id = id;
    }
    public void setId(int id) {
        this._id = id;
    }
    public int getId() {
        return this._id;
    }

    public void setState(int state) {
        this._state = state;
    }
    public int getState() {
        return this._state;
    }
    public bool isState()
    {
        return this._state != 0;
    }

    public void setBaseEffectValue(IBaseEffectValue value) {
        int index = this._baseEffectValues.FindIndex(item => item.getType() == value.getType());
        if (index != -1) {
            this._baseEffectValues.RemoveAt(index);
        }
        this._baseEffectValues.Add(value);
    }

    public List<IBaseEffectValue> getBaseEffectValues() {
        return this._baseEffectValues;
    }

    public IBaseEffectValue getBaseEffectValue(BaseEffectType type) {
        IBaseEffectValue baseEffectValue =  this._baseEffectValues.Find(item => item.getType() == type);
        if (baseEffectValue == null) {
            baseEffectValue = this._baseEffectValueInstance(type);
            this._baseEffectValues.Add(baseEffectValue);
        }
        return baseEffectValue;
    }

    public void setBaseEffectValueInstance(Func<BaseEffectType, IBaseEffectValue> func) {
        this._baseEffectValueInstance = func;
    }

}
