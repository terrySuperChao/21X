using System.Collections.Generic;

[System.Serializable]
public class BasePartInfo : IPart
{
    public int id; 
    public string name;
    public int type;
    public int value;
    public string desc;
    public List<int> partIds;

    public int getId() {
        return this.id;
    }

    public string getName() { 
        return this.name;
    }

    public int getType() { 
        return this.type;
    }

    public int getValue() {
        return this.value;
    }

    public string getDesc() {
        return this.desc;
    }

    public TargetPart getTargetPart() { 
        return TargetPart.basePart;
    }

    public List<int> getPartIds() {
        return this.partIds;
    }
}

public class BasePartConfig
{
    private readonly string _path = "config/basePart";
    private List<BasePartInfo> _list = null;
    public void init()
    {
        this._list = JsonMgr.Instance.readObject<List<BasePartInfo>>(this._path);
    }

    public List<BasePartInfo> getBasePart() {
        return this._list;
    }

    public BasePartInfo getBasePartId(int id) { 
        return this._list.Find(x => x.getId() == id);
    }
}
