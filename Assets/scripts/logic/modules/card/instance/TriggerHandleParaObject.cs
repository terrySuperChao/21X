public class TriggerHandleParaObject : ITriggerHandlePara
{
    private IAssembleCard _card = null;
    private IUser _attackUser = null;
    private IUser _defenseUser = null;
    private PokerSuit _pokerSuit = PokerSuit.club;
    private bool _isMagicAttack = false;
    private IGameSettlePara _gameSettlePara = null;

    public TriggerHandleParaObject()
    {
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

    public void setPokerSuit(PokerSuit pokerSuit) {
        this._pokerSuit = pokerSuit;
    }
    public PokerSuit getPokerSuit() {
        return this._pokerSuit;
    }

    public void setMagicAttack(bool isMagicAttack)
    {
        this._isMagicAttack = isMagicAttack;
    }
    public bool isMagicAttack()
    {
        return this._isMagicAttack;
    }

    public IAssembleCard getAssembleCard()
    {
        return this._card;
    }
    public void setAssembleCard(IAssembleCard card) {
        this._card = card;
    }

    public void setGameSettlePara(IGameSettlePara para) {
        this._gameSettlePara = para;
    }
    public IGameSettlePara getGameSettlePara() {
        return this._gameSettlePara;
    }

    public void reset() {
        if (this._gameSettlePara != null) {
            this._gameSettlePara.reset();
        }
    }
}
