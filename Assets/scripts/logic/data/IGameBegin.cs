using System.Collections.Generic;

public interface IGameBeginPara {
    public List<IUser> getUsers();
}

public interface IGameBegin
{
    public void gameBegin(IGameBeginPara para);
}
