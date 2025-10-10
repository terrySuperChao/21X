using System;
public class CardHandleParaObject : ICardHandlePara
{
    private ICard _card;
    private IUser _user;
    private IUser _attackUser;
    private IUser _defenseUser;
    private IPoker _poker;
    private float _baseValue;
    private IRoundResult _roundResult;

    public CardHandleParaObject()
    {
    }

    public IUser getUser() {
        return _user;
    }
    public void setUser(IUser user) { 
        _user = user;
    }

    public void setCard(ICard card) {
        _card = card;
    }
    public ICard getCard() {
        return _card;
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
}
