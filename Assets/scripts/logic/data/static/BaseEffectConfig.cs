using System.Collections.Generic;

[System.Serializable]
public class BaseEffectInfo : IPart
{
    public int ID = -1; 
    public string Name = "";
    public int Quality = 0;
    public int Profession = 0;
    public string Belong_Base = "";
    public string Action_Genre = "";
    public float Value_Default = 0;
    public float Value_Upgrade = 0;
    public string Correspond_Advanced = ""; 
    public string Link = "";
    public string Description = "";

    public int getId() {
        return this.ID;
    }

    public string getName() { 
        return this.Name;
    }

    public int getProfession()
    {
        return this.Profession;
    }

    public int getTriggerEvent()
    {
        return 0;
    }

    public string getDesc() {
        return this.Description;
    }

    public string getImage()
    {
        return "UI/pokers/blt_game_poker_01_2_01";
    }

    public string getBelongBase()
    {
        return this.Belong_Base;
    }

    public string getCorrespondAdvanced()
    {
        return this.Correspond_Advanced;
    }

    public string getCorrespondBase()
    {
        return "";
    }

    public float getValueDefault()
    {
        return this.Value_Default;
    }

    public float getValueUpgrade()
    {
        return this.Value_Upgrade;
    }

    public string getLogic()
    {
        return "";
    }

    public string getActionGenre()
    {
        return this.Action_Genre;
    }

    public TargetPart getTargetPart() { 
        return TargetPart.baseEffect;
    }
}

public class BaseEffectConfig
{
    private readonly string _path = "config/baseEffect";
    private List<BaseEffectInfo> _list = null;
    public void init()
    {
        this._list = JsonMgr.Instance.readObject<List<BaseEffectInfo>>(this._path);
    }

    public List<BaseEffectInfo> getBaseEffect() {
        return this._list;
    }

    public BaseEffectInfo getBaseEffectId(int id) {
        BaseEffectInfo info = this._list.Find(x => x.getId() == id);
        if (info == null) {
            info = new BaseEffectInfo();
        }
        return info;
    }
}
