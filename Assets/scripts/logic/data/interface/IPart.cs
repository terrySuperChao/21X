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

    public List<float> getValueDefault();

    public List<float> getValueUpgrade();

    public string getLogic();

    public TargetPart getTargetPart();
}

