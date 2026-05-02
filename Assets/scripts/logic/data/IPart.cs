using System.Collections.Generic;

public interface IPart
{
    public int getId();

    public string getName();

    public string getDesc();

    public string getImage();

    public int getProfession();

    public string getBelongBase();

    public string getCorrespondBase();

    public float getValueDefault();

    public float getValueUpgrade();

    public TargetPart getTargetPart();
}

