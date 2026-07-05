public class CardHandleParaObject : ICardHandlePara
{
    private IAssembleCard _card;
    private IUser _attackUser;
    private IUser _defenseUser;
    public CardHandleParaObject()
    {
    }
    public void setCard(ICard card) {
        
    }
    public ICard getCard() {
        return null;
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

    public void setAssembleCard(IAssembleCard card) {
        this._card = card;
    }

    public IAssembleCard getAssembleCard() {
        return this._card;
    }
}
