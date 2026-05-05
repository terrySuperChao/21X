public class TriggerHandleParaObject : ITriggerHandlePara
{
    private IAssembleCard _card;
    private IUser _user;
    private IUser _attackUser;
    private IUser _defenseUser;
    private IPoker _poker;
    private bool _isBlackJock;
    private bool _isMagicAttack;
    private float _baseValue;
    private IRoundResult _roundResult;

    public TriggerHandleParaObject()
    {
    }

    public IUser getUser() {
        return _user;
    }
    public void setUser(IUser user) { 
        _user = user;
    }

    public void setAttackUser(IUser user)
    {
        _attackUser = user;
    }

    public IUser getAttackUser() {
        return _attackUser;
    }

    public void setDefenseUser(IUser user)
    {
        _defenseUser =  user;
    }
    public IUser getDefenseUser() {
        return _defenseUser;
    }

    public IPoker getPoker() {
        return _poker;
    }
    public void setPoker(IPoker poker) {
        _poker = poker;
    }

    public float getBaseValue() {
        return _baseValue;
    }
    public void setBaseValue(float value) {
        _baseValue = value;
    }

    public IRoundResult getRoundResult() {
        return _roundResult;
    }
    public void setRoundResult(IRoundResult value) {
        _roundResult = value;
    }


    public IAssembleCard getAssembleCard()
    {
        return this._card;
    }
    public void setAssembleCard(IAssembleCard card) {
        this._card = card;
    }

    public void setBlackJock(bool isBlackJock) {
        this._isBlackJock = isBlackJock;
    }

    public bool isBlackJock()
    {
        return this._isBlackJock;
    }

    public void setMagicAttack(bool isMagicAttack) {
        this._isMagicAttack = isMagicAttack;
    }
    public bool isMagicAttack()
    {
        return this._isMagicAttack;
    }
}
