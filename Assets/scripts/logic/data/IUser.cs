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

    public bool isNpc();

}
