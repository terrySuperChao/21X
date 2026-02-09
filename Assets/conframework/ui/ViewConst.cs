using System.Collections.Generic;

[System.Serializable]
public class IViewInfo
{
    public string name;
    public string resPath;
    public int viewType;
    public string desc;
}

public enum ViewType
{
    none,
    view,
    alert,
    tip,
}


[System.Serializable]
public class LangInfo
{ 
    public string name;
    public string zh_CN;
    public string zh_SG;
	public string zh_TW;
	public string zh_HK;
	public string zh_MO;
	public string en_US;
	public string en_GB;
	public string ar_SA;
	public string vi_VN;
    public string id_ID;
	public string th_TH;
}
