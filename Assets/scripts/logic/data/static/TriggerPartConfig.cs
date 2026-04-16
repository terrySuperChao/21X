using System.Collections.Generic;

[System.Serializable]
public class TriggerPartInfo:IPart
{
    public int id;
    public string name;
    public int type;
    public int value;
    public string desc;
    public List<int> partIds;
    public int getId()
    {
        return this.id;
    }

    public string getName()
    {
        return this.name;
    }

    public int getLevel()
    {
        return 0;
    }

    public int getType()
    {
        return this.type;
    }

    public int getValue()
    {
        return this.value;
    }

    public string getDesc()
    {
        return this.desc;
    }

    public TargetPart getTargetPart()
    {
        return TargetPart.triggerPart;
    }

    public List<int> getPartIds()
    {
        return this.partIds;
    }
}

public class TriggerPartConfig
{
    private readonly string _path = "config/triggerPart";
    private List<TriggerPartInfo> _list = null;
    public void init()
    {
        this._list = JsonMgr.Instance.readObject<List<TriggerPartInfo>>(this._path);
    }

    public List<TriggerPartInfo> getTriggerPart() {
        return this._list;
    }

    public TriggerPartInfo getTriggerPartId(int id) { 
        return this._list.Find(x => x.getId() == id);
    }
}
