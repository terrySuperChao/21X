using System.Collections.Generic;

[System.Serializable]
public class AdvancedEffectInfo : IPart
{
    public int ID = -1; 
    public string Name = "";
    public int Quality = 0;
    public int Profession = 0;
    public string Belong_Advanced = "";
    public string Action_Genre = "";  
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

    public int getTriggerEvent() {
        return 0;
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
        return this.Belong_Advanced;
    }

    public string getCorrespondAdvanced() {
        return "";
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

    public string getLogic()
    {
        return "";
    }

    public string getActionGenre()
    {
        return this.Action_Genre;
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
        AdvancedEffectInfo info = this._list.Find(x => x.getId() == id);
        if (info == null) {
            info = new AdvancedEffectInfo();
        }
        return info;
    }
}
