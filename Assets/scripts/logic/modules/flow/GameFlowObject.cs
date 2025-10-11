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

public class GameSettlePara : GameFlowPara, IGameSettlePara
{
    private int _winIndex;
    private bool _isBackJock;
    public GameSettlePara(List<IUser> users, int winIndex, bool isBackJock) : base(users)
    {
        _winIndex = winIndex;
        _isBackJock = isBackJock;
    }

    public int getWinIndex()
    {
        return _winIndex;
    }

    public bool isBackJock()
    {
        return _isBackJock;
    }
}




public class GameFlowObject : IGameFlow
{
    public void gameBegin(IGameBeginPara para) {
        _gameBegin(para);
    }

    public void handPokerAfter(IHandPokerAfterPara para)
    {
        _handPokerAfter(para);
    }

    public void dealPokerAfter(IDealPokerAfterPara para) {
        _dealPokerAfter(para);
    }
    public bool gameSettle(IGameSettlePara para) {
        return _gameSettle(para);
    }

    protected virtual void _gameBegin(IGameBeginPara para){}

    protected virtual void _handPokerAfter(IHandPokerAfterPara para){}

    protected virtual void _dealPokerAfter(IDealPokerAfterPara para){}
    protected virtual bool _gameSettle(IGameSettlePara para)
    {
        return false;
    }

    public List<int> getPokerValue(List<IPoker> pokers)
    {
        List<int> values = new List<int>();
        List<IPoker> APokers = new List<IPoker>();
        for (int i = 0; i < pokers.Count; i++)
        {
            if (pokers[i].getRank() == 14)
            {
                APokers.Add(pokers[i]);
                values.Add(0);
            }
            else if (pokers[i].getRank() == 10 ||
                      pokers[i].getRank() == 11 ||
                      pokers[i].getRank() == 12 ||
                      pokers[i].getRank() == 13)
            {
                values.Add(10);
            }
            else
            {
                values.Add(pokers[i].getRank());
            }
        }

        int remainPoint = 21;
        for (int j = 0; j < values.Count; j++)
        {
            remainPoint -= values[j];
        }

        for (int i = 0; i < APokers.Count; i++)
        {
            int value = 0;
            if (remainPoint >= 11 && remainPoint - 11 >= ((APokers.Count - 1) - i))
            {
                value = 11;
            }
            else
            {
                value = 1;
            }
            remainPoint -= value;

            for (int j = 0; j < values.Count; j++)
            {
                if (values[j] == 0)
                {
                    values[j] = value;
                    break;
                }
            }
        }

        return values;
    }
}
