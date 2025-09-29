public class UIPokerPara : IUIPokerPara
{
    private IUser _attackUser;
    private IPoker _poker;
    private float _addValue;
    private float _finalValue;
    private bool _isBackJock;

    public UIPokerPara(IUser attackUser, IPoker poker, float addValue,float finalValue,bool isBackJock)
    {
        _attackUser = attackUser;
        _poker = poker;
        _addValue = addValue;
        _finalValue = finalValue;
        _isBackJock = isBackJock;
    }

    public float getAddValue() {
        return _addValue;
    }
    public float getFinalValue() {
        return _finalValue;
    }

    public IUser getAttackUser()
    {
        return _attackUser;
    }

    public IPoker getPoker()
    {
        return _poker;
    }

    public bool isBackJock()
    {
        return _isBackJock;
    }
}
