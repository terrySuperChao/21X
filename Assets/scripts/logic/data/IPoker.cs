public interface IPoker
{
    public int getId();
    public void setId(int id);
    public int getValue();
    public void setValue(int value);

    public int getRank();
    public int getSuit();

    public bool isBack();

    public void setBack(bool isBack);
}
