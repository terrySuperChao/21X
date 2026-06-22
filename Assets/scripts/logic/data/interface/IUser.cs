using Pb;

public interface IUser
{
    public string getUserId();
    public int getMoney();

    public void setMoney(int value);
    public void addMoney(int value);

    public void setMaxBlood(float value);
    public float getMaxBlood();

    public void setBlood(float value);
    public float addBlood(float value);
    public float getBlood();

    public void setAttack(float value);
    public float addAttack(float value);
    public float getAttack();

    public void setDefense(float value);
    public float addDefense(float value);
    public float getDefense();

    public void setMaxMagic(float value);
    public void addMaxMagic(float value);
    public float getMaxMagic();

    public void setMagic(float value);
    public float addMagic(float value);
    public float getMagic();

    public bool isNpc();
    public void reset();

    public void setState(UserState state);
    public UserState getState();

    //下一次
    public void setExtraInfo(IExtraInfo extra);
    public IExtraInfo getExtraInfo();
}
