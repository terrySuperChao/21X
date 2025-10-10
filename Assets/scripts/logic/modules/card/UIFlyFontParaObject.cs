public class UIFlyFontParaObject : IUIFlyFontPara
{
    private IUser _user;
    private ICard _card;
    private string _text;
    public UIFlyFontParaObject(IUser user, ICard card,string text) {
        _user = user;
        _card = card;
        _text = text;
    }
    public IUser getUser() {
        return _user;
    }
    public ICard getCard() {
        return _card;
    }

    public string getText() {
        return _text;
    }
}
