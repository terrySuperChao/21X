public class UIPokerPara : IUIPokerPara
{
    private IUser _user;
    private IPoker _poker;
    private float _finalValue;
    private string _text;
    
    public UIPokerPara(IUser user, IPoker poker, float finalValue, string text)
    {
        _user = user;
        _poker = poker;
        _finalValue = finalValue;
        _text = text;
    }

    public IUser getUser()
    {
        return _user;
    }

    public IPoker getPoker()
    {
        return _poker;
    }

    public float getFinalValue()
    {
        return _finalValue;
    }

    public string getText()
    {
        return _text;
    }
}
