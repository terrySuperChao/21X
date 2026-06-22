public class UIBuffParaObject : IUIBuffPara
{
    private IUser _user;
    private BuffType _buffType;
    public UIBuffParaObject(IUser user, BuffType buffType) {
        this._user = user;
        this._buffType = buffType;
    }
    public IUser getUser() {
        return this._user;
    }
    public BuffType getBuffType() {
        return this._buffType;
    }
}
