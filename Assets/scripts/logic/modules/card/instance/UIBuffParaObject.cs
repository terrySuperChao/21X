public class UIBuffParaObject : IUIBuffPara
{
    private IUser _user;
    private BaseEffectType _buffType;
    public UIBuffParaObject(IUser user, BaseEffectType buffType) {
        this._user = user;
        this._buffType = buffType;
    }
    public IUser getUser() {
        return this._user;
    }
    public BaseEffectType getBuffType() {
        return this._buffType;
    }
}
