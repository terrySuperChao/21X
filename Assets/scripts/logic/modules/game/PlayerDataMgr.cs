using Pb;
public class PlayerDataMgr : Singleton<PlayerDataMgr>
{
    private Player _player;
    public Player newPlayer() {
        Player player = new Player();
        return player;
    }

    public void deserialized(GameData data) {
        this._player = data.Player;
    }

    public void serialized(GameData data) {

    }

    public int getMoney() {
        return this._player.Money;
    }

    public void setMoney(int value) {
        this._player.Money = value;
    }

    public void addMoney(int value) {
        this._player.Money += value;
    }

    public int getDiamond()
    {
        return this._player.Diamond;
    }

    public void setDiamond(int value)
    {
        this._player.Diamond = value;
    }

    public void addDiamond(int value)
    {
        this._player.Diamond += value;
    }


    public int getHP()
    {
        return this._player.Hp;
    }

    public void setHP(int value)
    {
        this._player.Hp = value;
    }

    public void addHP(int value)
    {
        this._player.Hp += value;
        this._player.Hp = this._player.Hp > this._player.MaxHP ? this._player.MaxHP : this._player.Hp;
    }

    public int getMaxHP()
    {
        return this._player.MaxHP;
    }

    public void setMaxHP(int value)
    {
        this._player.MaxHP = value;
    }

    public void addMaxHP(int value)
    {
        this._player.MaxHP += value;
    }

    public int getMagic()
    {
        return this._player.Magic;
    }

    public void setMagic(int value)
    {
        this._player.Magic = value;
    }

    public void addMagic(int value)
    {
        this._player.Magic += value;
        this._player.Magic = this._player.Magic > this._player.MaxMagic ? this._player.MaxMagic : this._player.Magic;
    }

    public int getMaxMagic()
    {
        return this._player.MaxMagic;
    }

    public void setMaxMagic(int value)
    {
        this._player.MaxMagic = value;
    }

    public void addMaxMagic(int value)
    {
        this._player.MaxMagic += value;
    }

    public int getRoleId() { 
        return this._player.RoleId;
    }

    public void setRoleId(int value) { 
        this._player.RoleId = value;
    }
}
