using System.Collections.Generic;

[System.Serializable]
public class TriggerInfo:IPart
{
    public int ID = -1;
    public string Name = "";
    public int Quality = 0;
    public int Priority = 0;
    public int Profession = 0;
    public int Trigger = 0;
    public string Logic = "";
    public string Correspond_Base = "";
    public string Description = "";
    public string Remark = "";
    public int getId()
    {
        return this.ID;
    }

    public string getName()
    {
        return this.Name;
    }

    public int getProfession()
    {
        return this.Profession;
    }

    public int getTriggerEvent()
    {
        return this.Trigger;
    }

    public string getDesc()
    {
        return this.Description;
    }

    public string getImage()
    {
        return "UI/game/plb_game_btn_05";
    }

    public string getBelongBase()
    {
        return "";
    }

    public string getCorrespondAdvanced()
    {
        return "";
    }

    public string getCorrespondBase()
    {
        return this.Correspond_Base;
    }

    public List<float> getValueDefault()
    {
        return null;
    }

    public List<float> getValueUpgrade()
    {
        return null;
    }

    public string getLogic()
    {
        return this.Logic;
    }

    public TargetPart getTargetPart()
    {
        return TargetPart.trigger;
    }
}

public class TriggerConfig
{
    private readonly string _path = "config/Trigger";
    private List<TriggerInfo> _list = null;
    public void init()
    {
        this._list = JsonMgr.Instance.readObject<List<TriggerInfo>>(this._path);
    }

    public List<TriggerInfo> getTrigger() {
        return this._list;
    }

    public TriggerInfo getTriggerId(int id) {
        TriggerInfo info = this._list.Find(x => x.getId() == id);
        if (info == null) {
            info = new TriggerInfo();
        }
        return info;
    }
}
