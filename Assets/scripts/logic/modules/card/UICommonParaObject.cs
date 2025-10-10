public class UICommonParaObject: IUICommonPara
{
    private IUser _user;
    private ValueType _type;
    private float _value;
    private float _finalValue;
    public UICommonParaObject(IUser user, ValueType type,float value,float finalValue) {
        _user = user;
        _type = type;
        _value = value;
        finalValue = _finalValue;
    }
    public IUser getUser() {
        return _user;
    }

    public ValueType getValueType() {
        return _type;
    }

    public float getValue()
    {
        return _value;
    }

    public float getFinalValue() {
        return _finalValue;
    }
}
