public class BaseEffectValueObject : IBaseEffectValue
{
    private BaseEffectType _type;
    private float _value = 0;
    private float _maxValue = float.MaxValue;
   
    public void setType(BaseEffectType type) {
        this._type = type;
    }

    public BaseEffectType getType() {
        return this._type;
    }

    public void setMaxValue(float value)
    {
        this._maxValue = value;
    }

    public float getMaxValue() {
        return this._maxValue;
    }

    public void setValue(float value) {
        this._value = value;
        this._value = this._value > this._maxValue ? this._maxValue : this._value;
    }
    public void addValue(float value) {
        this._value += value;
        this._value = this._value < 0 ? 0 : this._value;
        this._value = this._value > this._maxValue ? this._maxValue : this._value;
    }

    public float getValue() {
        return this._value;
    }

    public void clearValue() {
        this._value = 0;
    }
}
