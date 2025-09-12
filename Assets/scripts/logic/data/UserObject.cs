using System.Diagnostics;

public class UserObject : IUser
{
    private static int global_id = 0;
    private string _userId = "";
    private int _money = 0;
    private int _playCount = 0;//总次数
    private int _wins = 0; //胜场
    private float _blood = 0;
    private float _maxBlood = 0;
    private float _attack = 0;
    private float _defense = 0;
    private float _magic = 0;
    private float _maxMagic = 0;
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

    public void setBlood(float value) {
        _blood = value;
    }

    public void setMaxBlood(float value) {
        _maxBlood = value;
    }

    public float getMaxBlood() {
        return _maxBlood;
    }

    public void addBlood(float value) {
        _blood += value;
        if (_blood > _maxBlood) {
            _blood = _maxBlood;
        }
        if (_blood < 0) {
            _blood = 0;
        }
    }

    public float getBlood() {
        return _blood;
    }

    public void setAttack(float value) {
        _attack = value;
    }
    public void addAttack(float value) {
        _attack += value;
    }
    public float getAttack() {
        return _attack;
    }

    public void setDefense(float value) {
        _defense = value;
    }

    public void addDefense(float value) {
        _defense += value;
    }

    public float getDefense() {
        return _defense;
    }

    public void setMaxMagic(float value) {
        _maxMagic = value;
    }

    public float getMaxMagic() {
        return _maxMagic;
    }

    public void addMagic(float value) {
        _magic += value;
        if (_magic > _maxMagic) {
            _magic = _maxMagic;
        }
        if (_magic < 0)
        {
            _magic = 0;
        }
    }

    public float getMagic() {
        return _magic;
    }

    public void reset() {
        _money = 0;
        _playCount = 0;//总次数
        _wins = 0; //胜场
        _blood = 0;
        _attack = 0;
        _defense = 0;
        _magic = 0;
    }
}
