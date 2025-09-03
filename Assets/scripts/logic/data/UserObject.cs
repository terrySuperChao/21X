public class UserObject : IUser
{
    private static int global_id = 0;
    private string _userId = "";
    private int _money = 0;
    private int _playCount = 0;//总次数
    private int _wins = 0; //胜场
    private bool _isNpc = false;

    public UserObject(bool isNpc) {
        _isNpc = isNpc;
        _userId = (++global_id).ToString();
    }

    public string getUserId() {
        return _userId;
    }

    public int getMoney() {
        return _money;
    }

    public void setMoney(int value) {
        _money = value;
    }

    public void addMoney(int value) {
        _money += value;
    }

    public void addPlay() {
        _playCount++;
    }
    public int getWins() {
        return _wins;
    }

    public void addWins() {
        _wins++;
    }

    public double getWinRate()
    {
        if (_playCount == 0)
        {
            return 0;
        }
        else
        {
            return _wins * 1.0 / _playCount;
        }
    }

    public bool isNpc() {
        return _isNpc;
    }
}
