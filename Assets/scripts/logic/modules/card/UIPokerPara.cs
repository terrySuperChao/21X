public class UIPokerPara : IUIPokerPara
{
    private IUser _user;
    private IPoker _poker;
    private float _addValue;
    private float _finalValue;
    private float _mult;
    private bool _isBackJock;
    
    public UIPokerPara(IUser user, IPoker poker, float addValue,float finalValue,float mult,bool isBackJock)
    {
        _user = user;
        _poker = poker;
        _addValue = addValue;
        _finalValue = finalValue;
        _mult = mult;
        _isBackJock = isBackJock;
    }

    public float getAddValue() {
        return _addValue;
    }
    public float getFinalValue() {
        return _finalValue;
    }

    public IUser getUser()
    {
        return _user;
    }

    public IPoker getPoker()
    {
        return _poker;
    }

    public float getMult() {
        return _mult;
    }

    public bool isBackJock()
    {
        return _isBackJock;
    }
}
