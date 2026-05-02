using System.Collections.Generic;

[System.Serializable]
public class BaseEffectInfo : IPart
{
    public int ID; 
    public string Name;
    public int Quality;
    public int Profession;
    public string Belong_Base;
    public string Action_Genre;
    public float Value_Default;
    public float Value_Upgrade;
    public string Correspond_Advanced;
    public string Link;
    public string Description;

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
        return this._list.Find(x => x.getId() == id);
    }
}
