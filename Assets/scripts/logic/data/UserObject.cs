
using Pb;

public class UserObject : IUser
{
    private static int global_id = 0;
    private string _userId = "";
    private int _money = 0;
    private int _playCount = 0;
    private int _wins = 0;
    private float _blood = 0;
    private float _maxBlood = 0;
    private float _attack = 0;
    private float _defense = 0;
    private float _magic = 0;
    private float _maxMagic = 0;
    private int _point = 0;
    private bool _isNpc = false;

    private IExtraInfo _extra = null;
    private UserState _state = UserState.none;

    public UserObject(bool isNpc) {
        this._isNpc = isNpc;
        this._userId = (++global_id).ToString();
    }

    public string getUserId() {
        return this._userId;
    }

    public int getMoney() {
        return this._money;
    }

    public void setMoney(int value) {
        this._money = value;
    }

    public void addMoney(int value) {
        this._money += value;
    }

    public void addPlay() {
        this._playCount++;
    }
    public int getWins() {
        return this._wins;
    }

    public void addWins() {
        this._wins++;
    }

    public double getWinRate()
    {
        if (this._playCount == 0)
        {
            return 0;
        }
        else
        {
            return this._wins * 1.0 / this._playCount;
        }
    }

    public bool isNpc() {
        return this._isNpc;
    }

    public void setBlood(float value) {
        this._blood = GameUtils.getNumberDigits(value);
    }

    public void setMaxBlood(float value) {
        this._maxBlood = GameUtils.getNumberDigits(value);
    }

    public float getMaxBlood() {
        return this._maxBlood;
    }

    public float addBlood(float value) {
        this._blood += value;
        this._blood = this._blood > this._maxBlood ? this._maxBlood : this._blood;
        this._blood = this._blood < 0 ? 0 : GameUtils.getNumberDigits(this._blood);
        return this._blood;
    }

    public float getBlood() {
        return this._blood;
    }

    public void setAttack(float value) {
        this._attack = GameUtils.getNumberDigits(value);
    }
    public float addAttack(float value) {
        this._attack += value;
        this._attack = this._attack < 0 ? 0 : GameUtils.getNumberDigits(this._attack);
        return _attack;
    }
    public float getAttack() {
        return _attack;
    }

    public void setDefense(float value) {
        this._defense = GameUtils.getNumberDigits(value);
    }

    public float addDefense(float value) {
        this._defense += value;
        this._defense = this._defense < 0 ? 0 : GameUtils.getNumberDigits(this._defense);
        return _defense;
    }

    public float getDefense() {
        return this._defense;
    }

    public void setMaxMagic(float value) {
        this._maxMagic = GameUtils.getNumberDigits(value);
    }

    public float getMaxMagic() {
        return this._maxMagic;
    }

    public float addMagic(float value) {
        this._magic += value;
        this._magic = this._magic > this._maxMagic ? this._maxMagic : this._magic;
        this._magic = this._magic < 0 ? 0 : GameUtils.getNumberDigits(this._magic);
        return _magic;
    }

    public void setMagic(float value)
    {
        this._magic = GameUtils.getNumberDigits(value);
    }

    public float getMagic() {
        return this._magic;
    }

    public void reset() {
        this._money = 0;
        this._playCount = 0;
        this._wins = 0;
        this._blood = 0;
        this._attack = 0;
        this._defense = 0;
        this._magic = 0;
        this._state = UserState.none;
    }

    public void setState(UserState state) { 
        this._state = state;
    }

    public UserState getState() {
        return _state;
    }

    public void setPoint(int value) { 
        this._point = value;
    }

    public int getPoint() {
        return this._point;
    }

    public void setExtraInfo(IExtraInfo extra) {
        this._extra = extra;
    }

    public IExtraInfo getExtraInfo() {
        return this._extra;
    }
}
