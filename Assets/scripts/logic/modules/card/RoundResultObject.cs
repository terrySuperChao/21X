public class RoundResultObject:IRoundResult
{
    private float _penetrate = 0;
    private float _saveMagic = 0;
    private float _saveAttack = 0;
    private float _reflectValue = 0;
    private float _attributeMult = 1;
    //´©Í¸
    public float getPenetrateValue() {
        return _penetrate;
    }

    public void setPenetrateValue(float value)
    {
        _penetrate = value;
    }

    //±£´æÄ§·¨
    public float getSaveMagicValue() {
        return _saveMagic;
    }

    public void setSaveMagicValue(float value)
    {
        _saveMagic = value;
    }

    //±£´æ¹¥»÷Á¦
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
}
