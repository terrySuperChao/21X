using System.Collections.Generic;

public interface IGameFlowPara
{
    public List<IUser> getUsers();
}

public interface IGameSettlePara : IGameFlowPara
{
    public int getWinIndex();
    public void setWinIndex(int winIndex);
    public bool isBlackJack();
    public void setBlackJack(bool blackJack);
    public void reset();
}

public interface IHandPokerBeforePara : IGameFlowPara
{
}

public interface IHandPokerAfterPara : IGameFlowPara
{
}

public interface IDealPokerAfterPara : IGameFlowPara
{
    public IUser getUser();
}

public interface IStopPokerAfterPara : IGameFlowPara
{
    public IUser getUser();
}

public interface IGameBeginPara : IGameFlowPara
{
}

public interface IAddCardAfterPara : IGameFlowPara
{
}

public interface IGameFlow
{
    public void gameBegin(IGameBeginPara para);

    public void handPokerBefore(IHandPokerBeforePara para);
    public void handPokerAfter(IHandPokerAfterPara para);

    public void dealPokerAfter(IDealPokerAfterPara para);
    public void stopPokerAfter(IStopPokerAfterPara para);
    public bool gameSettle(IGameSettlePara para);
}
