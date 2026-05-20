using System.Collections.Generic;

public class TriggerHandleParaObject : ITriggerHandlePara
{
    private IAssembleCard _card = null;
    private IUser _user = null;
    private IUser _attackUser = null;
    private IUser _defenseUser = null;
    private IPoker _poker = null;
    private PokerSuit _pokerSuit = PokerSuit.club;
    private bool _isMagicAttack = false;
    private float _baseValue = -1;
    private List<IRoundResult> _roundResults = new List<IRoundResult>();
    private IGameSettlePara _gameSettlePara = null;

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

    public void setPokerSuit(PokerSuit pokerSuit) {
        this._pokerSuit = pokerSuit;
    }
    public PokerSuit getPokerSuit() {
        return this._pokerSuit;
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

    public IRoundResult getRoundResult(IUser user) {
        return this._roundResults.Find(res => res.getUser() == user);
    }

    public void addRoundResult(IRoundResult value) {
        int index = this._roundResults.FindIndex(res => res.getUser() == value.getUser());
        if (index == -1) {
            this._roundResults.Add(value);
        }
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
        
        foreach (IRoundResult result in this._roundResults){
            result.reset();
        }

        this._attackUser.getExtraInfo().clearArmorATK();
        this._defenseUser.getExtraInfo().clearArmorATK();

        //
        this._attackUser.getExtraInfo().clearRtMagicValue();
        this._attackUser.getExtraInfo().clearTemporaryArmor();
        this._attackUser.getExtraInfo().clearImmunityDeBuff();       
        this._defenseUser.getExtraInfo().clearRtMagicValue();
        this._defenseUser.getExtraInfo().clearTemporaryArmor();
    }
}
