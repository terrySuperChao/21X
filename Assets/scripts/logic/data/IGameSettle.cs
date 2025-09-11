using System.Collections.Generic;

public interface IGameSettlePara {
    public List<IUser> getUsers();
    public int getWinIndex();
    public bool isBackJock();
}

public interface IGameSettle
{
    public void gameSettle(IGameSettlePara para);
}
