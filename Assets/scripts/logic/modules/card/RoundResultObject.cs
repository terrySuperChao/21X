public class RoundResultObject:IRoundResult
{
    private IUser _user = null;
    private float _penetrate = 0;
    private float _saveMagic = 0;
    private float _saveAttack = 0;
    private float _reflectValue = 0;
    private float _magicValue = 0;
    private float _hurtValue = 0;
    private float _multATK = 0;
    private float _attributeMult = 1;

    public RoundResultObject(IUser user) {
        this._user = user;
    }

    public void setUser(IUser user) {
        this._user = user;
    }

    public IUser getUser() {
        return this._user;
    }

    //
    public float getPenetrateValue() {
        return _penetrate;
    }

    public void setPenetrateValue(float value)
    {
        _penetrate = value;
    }

    //
    public float getSaveMagicValue() {
        return _saveMagic;
    }

    public void setSaveMagicValue(float value)
    {
        _saveMagic += value;
    }

    //
    public float getSaveAttackValue() {
        return _saveAttack;
    }

    public void setSaveAttackValue(float value)
    {
        _saveAttack = value;
    }

    public float getReflectValue() {
        return _reflectValue;
    }
    public void setReflectValue(float value) {
        _reflectValue = value;
    }

    public float getAttributeMult() {
        return _attributeMult;
    }
    public void setAttributeMult(float value) {
        _attributeMult = value;
    }

    public float getMagicValue() {
        return this._magicValue;
    }
    public void addMagicValue(float value) {
        this._magicValue += value;
    }

    public float getHurtVaule() {
        return this._hurtValue;
    }
    public void addHurtValue(float value) {
        this._hurtValue += value;
    }

    public float getMultATK() {
        return this._multATK;
    }

    public void addMultATK(float value) {
        this._multATK += value;
    }

    public void reset() {
        this._penetrate = 0;
        this._saveMagic = 0;
        this._saveAttack = 0;
        this._reflectValue = 0;
        this._attributeMult = 1;
        this._hurtValue = 0;
        this._multATK = 0;
    }
}
