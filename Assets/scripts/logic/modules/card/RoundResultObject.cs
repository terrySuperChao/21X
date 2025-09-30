public class RoundResultObject:IRoundResult
{
    private float _penetrate;
    private float _saveMagic;
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
}
