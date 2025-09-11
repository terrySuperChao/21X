public interface IUser
{
    public string getUserId();
    public int getMoney();

    public void setMoney(int value);

    public void addMoney(int value);

    public void addPlay();

    public int getWins();

    public void addWins();

    public double getWinRate();

    public void setMaxBlood(float value);
    public void setBlood(float value);
    public void addBlood(float value);

    public float getBlood();

    public void addAttcak(float value);
    public float getAttack();

    public void addDefense(float value);
    public float getDefense();

    public void setMaxMagic(float value);
    public void addMagic(float value);

    public float getMagic();

    public bool isNpc();

    public void reset();

}
