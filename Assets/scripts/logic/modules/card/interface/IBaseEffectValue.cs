public interface IBaseEffectValue {
    public void setType(BaseEffectType type);
    public BaseEffectType getType();
    public void setValue(float value);
    public float getValue();
    public void addValue(float value);
    public void clearValue();
    public void setMaxValue(float value);
    public float getMaxValue();
}
