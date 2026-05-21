public class UIPokerPara : IUIPokerPara
{
    private IUser _user;
    private IPoker _poker;
    private float _value;
    private float _finalValue;
    private float _mult;
    
    public UIPokerPara(IUser user, IPoker poker,float value, float finalValue, float mult)
    {
        this._user = user;
        this._poker = poker;
        this._value = value;
        this._finalValue = finalValue;
        this._mult = mult;
    }

    public IUser getUser()
    {
        return _user;
    }

    public IPoker getPoker()
    {
        return _poker;
    }

    public float getValue() {
        return this._value;
    }

    public float getFinalValue()
    {
        return _finalValue;
    }

    public float getMult()
    {
        return this._mult;
    }
}
