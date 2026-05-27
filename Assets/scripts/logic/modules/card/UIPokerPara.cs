using System.Collections.Generic;

public class UIPokerPara : IUIPokerPara
{
    private IUser _user;
    private IPoker _poker;
    private List<IPoker> _pokers;
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
    public UIPokerPara(IUser user, List<IPoker> pokers, float value, float finalValue, float mult)
    {
        this._user = user;
        this._pokers = pokers;
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

    public List<IPoker> getPokers()
    {
        return this._pokers;
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
