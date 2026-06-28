using System.Collections.Generic;

[System.Serializable]
public class BaseEffectInfo : IPart
{
    public int ID = -1; 
    public string Name = "";
    public int Quality = 0;
    public int Profession = 0;
    public string Belong_Base = "";
    public string Value_Default = "";
    public string Value_Upgrade = "";
    public string Correspond_Advanced = ""; 
    public string Link = "";
    public string Description = "";
    private List<float> _valueDefaultArray = new List<float>();
    private List<float> _valueUpgradeArray = new List<float>();
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
        return "UI/game/plb_game_btn_04";
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

    public List<float> getValueDefault()
    {
        if (this._valueDefaultArray.Count == 0) {
            this.stringToList(this.Value_Default, this._valueDefaultArray);
        }
        return this._valueDefaultArray;
    }

    public List<float> getValueUpgrade()
    {
        if (this._valueUpgradeArray.Count == 0) {
            this.stringToList(this.Value_Upgrade, this._valueUpgradeArray);
        }
        return this._valueUpgradeArray;
    }

    public string getLogic()
    {
        return "";
    }

    public TargetPart getTargetPart() { 
        return TargetPart.baseEffect;
    }

    private void stringToList(string str, List<float> values) {
        string[] list = str.Split(",");
        for (int i = 0; i < list.Length; i++) {
            float addValue;
            float.TryParse(list[0], out addValue);
            values.Add(addValue);
        }
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
