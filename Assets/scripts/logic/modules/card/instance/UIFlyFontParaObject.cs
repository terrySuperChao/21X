public class UIFlyFontParaObject : IUIFlyFontPara
{
    private IUser _user;
    private ICard _card;
    private IAssembleCard _assembleCard;
    private BuffType _buffType;
    private string _text;
    public UIFlyFontParaObject(IUser user, ICard card,string text) {
        _user = user;
        _card = card;
        _text = text;
    }
    public UIFlyFontParaObject(IUser user, IAssembleCard card, string text) {
        this._user = user;
        this._text = text;
        this._assembleCard = card;
    }
    public UIFlyFontParaObject(IUser user, BuffType buffType, string text)
    {
        this._user = user;
        this._text = text;
        this._buffType = buffType;
    }
    public IUser getUser() {
        return _user;
    }
    public ICard getCard() {
        return _card;
    }
    public string getText()
    {
        return _text;
    }
    public IAssembleCard getAssembleCard() {
        return this._assembleCard;
    }
    public BuffType getBuffType()
    {
        return this._buffType;
    }
}
