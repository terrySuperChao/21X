using System.Collections.Generic;

public class GameFlowPara : IGameFlowPara
{
    private List<IUser> _users;
    public GameFlowPara(List<IUser> users)
    {
        _users = users;
    }
    public List<IUser> getUsers()
    {
        return _users;
    }
}

public class GameBeginPara : GameFlowPara, IGameBeginPara
{
    public GameBeginPara(List<IUser> users) : base(users)
    {

    }
}

public class AddCardAfterPara : GameFlowPara, IAddCardAfterPara 
{
    public AddCardAfterPara(List<IUser> users) : base(users)
    {

    }
}

public class HandPokerBeforePara : GameFlowPara, IHandPokerBeforePara
{
    public HandPokerBeforePara(List<IUser> users) : base(users)
    {

    }
}


public class HandPokerAfterPara : GameFlowPara, IHandPokerAfterPara
{
    public HandPokerAfterPara(List<IUser> users) : base(users)
    {

    }
}

public class DealPokerAfterPara : GameFlowPara, IDealPokerAfterPara
{
    private IUser _user;
    public DealPokerAfterPara(List<IUser> users, IUser user) : base(users)
    {
        _user = user;
    }
    public IUser getUser() {
        return _user;
    }
}

public class StopPokerAfterPara : GameFlowPara, IStopPokerAfterPara
{
    private IUser _user;
    public StopPokerAfterPara(List<IUser> users, IUser user) : base(users)
    {
        _user = user;
    }
    public IUser getUser()
    {
        return _user;
    }
}

public class GameSettlePara : GameFlowPara, IGameSettlePara
{
    private int _winIndex = -1;
    private bool _isBlackJack = false;
    public GameSettlePara(List<IUser> users,int winIndex,bool isBlackJack) : base(users)
    {
        this._winIndex = winIndex;
        this._isBlackJack = isBlackJack;
    }
    public int getWinIndex()
    {
        return this._winIndex;
    }

    public void setWinIndex(int winIndex) {
        this._winIndex = winIndex;
    }

    public bool isBlackJack() {
        return this._isBlackJack;
    }

    public void setBlackJack(bool blackJack) {
        this._isBlackJack = blackJack;
    }
    public void reset() {
        this._winIndex = -1;
        this._isBlackJack = false;
    }
}

public class GameFlowObject : IGameFlow
{
    public void gameBegin(IGameBeginPara para) {
        this._gameBegin(para);
    }

    public void handPokerBefore(IHandPokerBeforePara para)
    {
        this._handPokerBefore(para);
    }

    public void handPokerAfter(IHandPokerAfterPara para)
    {
        this._handPokerAfter(para);
    }

    public void dealPokerAfter(IDealPokerAfterPara para) {
        this._dealPokerAfter(para);
    }

    public void stopPokerAfter(IStopPokerAfterPara para)
    {
        this._stopPokerAfter(para);
    }

    public bool gameSettle(IGameSettlePara para) {
        return this._gameSettle(para);
    }

    protected virtual void _gameBegin(IGameBeginPara para){}
    protected virtual void _handPokerBefore(IHandPokerBeforePara para) { }
    protected virtual void _handPokerAfter(IHandPokerAfterPara para){}
    protected virtual void _dealPokerAfter(IDealPokerAfterPara para){}
    protected virtual void _stopPokerAfter(IStopPokerAfterPara para) { }
    protected virtual bool _gameSettle(IGameSettlePara para){ return false;}
}
