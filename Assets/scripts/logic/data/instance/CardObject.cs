
public class CardObject : ICard
{
    private int _id = 0;
    private int _type = 0;
    private int _level = 0;
    private string _name = "";
    private string _desc = "";

    public CardObject(int id,int type,int level,string name,string desc) {
        _id = id;
        _type = type;
        _level = level;
        _name = name;
        _desc = desc;
    }

    public string getDescript()
    {
        return _desc;
    }

    public string getName()
    {
        return _name;
    }

    public int getId()
    {
        return _id;
    }

    public int getLevel()
    {
        return _level;
    }

    public int getType()
    {
        return _type;
    }

    public void setDescript(string value)
    {
        _desc = value;
    }

    public void setName(string value)
    {
        _name = value;
    }

    public void setId(int value)
    {
        _id = value;
    }

    public void setLevel(int value)
    {
        _level = value;
    }

    public void setType(int value)
    {
        _type = value;
    }
}
