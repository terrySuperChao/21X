public class TriggerHandleParaObject : ITriggerHandlePara
{
    private IAssembleCard _card = null;
    private IUser _user = null;
    private IUser _attackUser = null;
    private IUser _defenseUser = null;
    private IPoker _poker = null;
    private bool _isMagicAttack = false;
    private float _baseValue = -1;
    private IRoundResult _roundResult = null;
    private IGameSettlePara gameSettlePara = null;

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

    public void setMagicAttack(bool isMagicAttack)
    {
        this._isMagicAttack = isMagicAttack;
    }
    public bool isMagicAttack()
    {
        return this._isMagicAttack;
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

    public void setGameSettlePara(IGameSettlePara para) {
        this.gameSettlePara = para;
    }
    public IGameSettlePara getGameSettlePara() {
        return this.gameSettlePara;
    }
}
