using System.Collections.Generic;
using UnityEngine;

public interface IDealCardPara {
    public IUser getUser();
    public ICard getCard();
}

public class DealCardPara : IDealCardPara{
    private IUser _user;
    private ICard _card;
    public DealCardPara(IUser user,ICard card) { 
        this._user = user;
        this._card = card;
    }
    public IUser getUser() { 
        return this._user; 
    }
    public ICard getCard()
    {
        return this._card;
    }
}

public interface ICandidacyCardPara
{
    public IUser getUser();
    public List<ICard> getCards();
}

public class CandidacyCardPara : ICandidacyCardPara
{
    private IUser _user;
    private List<ICard> _cards;
    public CandidacyCardPara(IUser user, List<ICard> cards)
    {
        this._user = user;
        this._cards = cards;
    }
    public IUser getUser()
    {
        return this._user;
    }
    public List<ICard> getCards()
    {
        return this._cards;
    }
}

public interface ICandidacyPartPara
{
    public int getTriggerId();
    public IUser getUser();
    public List<IPart> getParts();
}

public class CandidacyPartPara : ICandidacyPartPara
{
    private IUser _user;
    private List<IPart> _parts;
    private int _triggerId;
    public CandidacyPartPara(IUser user, List<IPart> parts, int triggerId)
    {
        this._user = user;
        this._parts = parts;
        this._triggerId = triggerId;
    }
    public IUser getUser()
    {
        return this._user;
    }
    public List<IPart> getParts()
    {
        return this._parts;
    }

    public int getTriggerId()
    {
        return this._triggerId;
    }
}


public interface IDealPokerPara
{
    public IUser getUser();
    public IPoker getPoker();
}

public class DealPokerPara : IDealPokerPara
{
    private IUser _user;
    private IPoker _poker;
    private int _point;
    private bool _isBlackJack;
    public DealPokerPara(IUser user, IPoker poker)
    {
        this._user = user;
        this._poker = poker;

    }
    public IUser getUser()
    {
        return this._user;
    }
    public IPoker getPoker()
    {
        return this._poker;
    }
}

public interface ISelectCardPara
{
    public IUser getUser();
    public ICard getCard();
    public Vector3 getPosition();
}

public class SelectCardPara : ISelectCardPara
{
    private IUser _user;
    private ICard _card;
    private Vector3 _position;
    public SelectCardPara(IUser user, ICard card, Vector3 position)
    {
        this._user = user;
        this._card = card;
        this._position = position;
    }
    public IUser getUser()
    {
        return this._user;
    }
    public ICard getCard()
    {
        return this._card;
    }

    public Vector3 getPosition()
    {
        return this._position;
    }
}

public interface ISelectPartPara
{
    public IUser getUser();
    public IPart getPart();
    public Vector3 getPosition();
}

public class SelectPartPara : ISelectPartPara
{
    private IUser _user;
    private IPart _part;
    private Vector3 _position;
    public SelectPartPara(IUser user, IPart part, Vector3 position)
    {
        this._user = user;
        this._part = part;
        this._position = position;
    }
    public IUser getUser()
    {
        return this._user;
    }
    public IPart getPart()
    {
        return this._part;
    }

    public Vector3 getPosition()
    {
        return this._position;
    }
}


public interface IRefactoringPara
{
    public IUser getUser();
    public int getNumber();
}

public class RefactoringPara : IRefactoringPara
{
    private IUser _user;
    private int _number;
    public RefactoringPara(IUser user, int number) {
        this._user = user;
        this._number = number;
    }

    public IUser getUser() { 
        return this._user;
    }
    public int getNumber() { 
        return this._number;
    }
}

public interface IReHandPokerPara
{
    public IUser getUser();
    public int getSuit();
}

public class ReHandPokerPara : IReHandPokerPara
{
    private IUser _user;
    private int _suit;
    public ReHandPokerPara(IUser user, int suit)
    {
        this._user = user;
        this._suit = suit;
    }

    public IUser getUser()
    {
        return this._user;
    }
    public int getSuit()
    {
        return this._suit;
    }
}

public interface ITotalHandPokerPointPara
{
    public IUser getUser();
    public int getPoint();
}

public class TotalHandPokerPointPara : ITotalHandPokerPointPara
{
    private IUser _user;
    private int _point;
    public TotalHandPokerPointPara(IUser user, int point)
    {
        this._user = user;
        this._point = point;
    }

    public IUser getUser()
    {
        return this._user;
    }
    public int getPoint()
    {
        return this._point;
    }
}





