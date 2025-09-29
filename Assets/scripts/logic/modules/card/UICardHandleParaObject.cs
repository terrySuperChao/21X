public class UICardHandleParaObject : CardHandleParaObject, IUICardHandlePara
{
    private float _addValue;
    private float _finalValue;
    public UICardHandleParaObject(ICardHandlePara para, float addValue,float finalValue) : base(para)
    {
        _addValue = addValue;
        _finalValue = finalValue;
    }

    public float getAddValue() {
        return _addValue;
    }
    public float getFinalValue() {
        return _finalValue;
    }

}
