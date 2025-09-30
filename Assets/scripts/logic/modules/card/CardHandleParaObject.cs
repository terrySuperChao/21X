using System;
public class CardHandleParaObject : ICardHandlePara
{
    private ICard _card;
    private IUser _attackUser;
    private IUser _defenseUser;
    private IPoker _poker;
    private float _baseValue;
    private IRoundResult _roundResult;
    public CardHandleParaObject(ICardHandlePara para)
    {
        _attackUser = para.getAttackUser();
        _defenseUser = para.getDefenseUser();
        _card = para.getCard();
        _poker = para.getPoker();
    }

    public CardHandleParaObject(IUser attackUser, IUser defenseUser, ICard card, IPoker poker) {
        _attackUser = attackUser;
        _defenseUser = defenseUser;
        _card = card;
        _poker = poker;
    }

    public void setCard(ICard card) {
        _card = card;
    }
    public ICard getCard() {
        return _card;
    }
    public IUser getAttackUser() {
        return _attackUser;
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
