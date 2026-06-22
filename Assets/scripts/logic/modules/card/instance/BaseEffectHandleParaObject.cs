public class BaseEffectHandleParaObject : IBaseEffectHandlePara
{
    private IUser _attackUser = null;
    private IUser _defenseUser = null;
    private AdvancedEffectType _type;
    private float _value = 0;
    
    public void setAttackUser(IUser user) {
        this._attackUser = user;
    }
    public IUser getAttackUser() {
        return this._attackUser;
    }

    public void setDefenseUser(IUser user) {
        this._defenseUser = user;
    }
    public IUser getDefenseUser() {
        return this._defenseUser;
    }

    public void setEffectType(AdvancedEffectType type) {
        this._type = type;
    }
    public AdvancedEffectType getEffectType() {
        return this._type;
    }

    public void setExtralValue(float value) {
        this._value = value;
    }
    public float getExtralValue() {
        return this._value;
    }
}
