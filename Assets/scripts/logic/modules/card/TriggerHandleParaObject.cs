public class TriggerHandleParaObject : ITriggerHandlePara
{
    private IAssembleCard _card;
    private IUser _user;
    private IUser _attackUser;
    private IUser _defenseUser;
    private IPoker _poker;
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

}
