using System.Collections.Generic;

[System.Serializable]
public class AdvancedEffectInfo : IPart
{
    public int ID; 
    public string Name;
    public int Quality;
    public int Profession;
    public string Belong_Base;
    public string Action_Genre;  
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
        return "UI/pokers/blt_game_poker_01_2_02";
    }

    public string getBelongBase()
    {
        return this.Belong_Base;
    }

    public string getCorrespondBase() {
        return "";
    }

    public float getValueDefault() {
        return 0.0f;
    }

    public float getValueUpgrade() {
        return 0.0f;
    }

    public TargetPart getTargetPart() { 
        return TargetPart.advancedEffect;
    }
}

public class AdvancedEffectConfig
{
    private readonly string _path = "config/AdvancedEffect";
    private List<AdvancedEffectInfo> _list = null;
    public void init()
    {
        this._list = JsonMgr.Instance.readObject<List<AdvancedEffectInfo>>(this._path);
    }

    public List<AdvancedEffectInfo> getAdvancedEffect() {
        return this._list;
    }

    public AdvancedEffectInfo getAdvancedEffectInfoId(int id) { 
        return this._list.Find(x => x.getId() == id);
    }
}
