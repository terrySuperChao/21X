using System.Collections.Generic;

public interface IPart
{
    public int getId();

    public string getName();

    public string getDesc();

    public string getImage();

    public int getProfession();

    public int getTriggerEvent();

    public string getBelongBase();

    public string getCorrespondAdvanced();

    public string getCorrespondBase();

    public float getValueDefault();

    public float getValueUpgrade();

    public string getLogic();

    public string getActionGenre();

    public TargetPart getTargetPart();
}

