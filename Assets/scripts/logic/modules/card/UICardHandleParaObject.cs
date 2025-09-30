public class UICardHandleParaObject : CardHandleParaObject, IUICardHandlePara
{
    private float _addValue;
    private float _finalValue;
    private PokerSuit _pokerSuit;
    public UICardHandleParaObject(ICardHandlePara para, float addValue,float finalValue,PokerSuit pokerSuit) : base(para)
    {
        _addValue = addValue;
        _finalValue = finalValue;
        _pokerSuit = pokerSuit;
    }

    public float getAddValue() {
        return _addValue;
    }
    public float getFinalValue() {
        return _finalValue;
    }

    public PokerSuit getPokerSuit() {
        return _pokerSuit;
    }

}
