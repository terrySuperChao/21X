//ÅÆ¶Ñ
using System.Collections.Generic;


public class GameBeginPara : IGameBeginPara
{
    private List<IUser> _users;
    public GameBeginPara(List<IUser> users) {
        _users = users;
    }
    public List<IUser> getUsers()
    {
        return _users;
    }
}
