using System.Collections.Generic;

public interface IPart
{
    public int getId();
    public string getName();
    public int getType();
    public int getValue();

    public string getDesc();
    public List<int> getPartIds();

    public TargetPart getTargetPart();
}

