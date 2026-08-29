using System;
using System.Collections.Generic;
using UnityEngine;
public class LanguageList
{
    public static string zh_CN = "zh_CN";
    public static string zh_SG = "zh_SG";
    public static string zh_TW = "zh_TW";
    public static string zh_HK = "zh_HK";
    public static string zh_MO = "zh_MO";
    public static string en_US = "en_US";
    public static string en_GB = "en_GB";
    public static string ar_SA = "ar_SA";
    public static string vi_VN = "vi_VN";
    public static string id_ID = "id_ID";
    public static string th_TH = "th_TH";
}

public class LangMgr : Singleton<LangMgr>
{
    private string _defLanguage = LanguageList.en_US;
    private string _curLanguageStr = LanguageList.en_US;
    private Dictionary<string, LangInfo> _configDic = null;
    private Dictionary<string, string> _languageMap = null;
    public void init(string path)
    {
        this._configDic = JsonMgr.Instance.readObject<Dictionary<string, LangInfo>>(path);
    }
    private bool isLanguageContains(string key)
    {
        if (key == null || key == "") {
            return false;
        }

        foreach (var value in this._configDic.Values) {
            if (this.getText(value, key) == "")
            {
                return false;
            }
            else {
                return true;
            }
        }
        return false;
    }

    private string getText(LangInfo info,string key) {
        if (info == null) return "";
        
        if (key == LanguageList.zh_CN)
        {
            return info.zh_CN;
        }
        else if (key == LanguageList.zh_SG)
        {
            return info.zh_SG;
        }
        else if (key == LanguageList.zh_TW)
        {
            return info.zh_TW;
        }
        else if (key == LanguageList.zh_HK)
        {
            return info.zh_HK;
        }
        else if (key == LanguageList.zh_MO)
        {
            return info.zh_MO;
        }
        else if (key == LanguageList.en_US)
        {
            return info.en_US;
        }
        else if (key == LanguageList.en_GB)
        {
            return info.en_GB;
        }
        else if (key == LanguageList.ar_SA)
        {
            return info.ar_SA;
        }
        else if (key == LanguageList.vi_VN)
        {
            return info.vi_VN;
        }
        else if (key == LanguageList.id_ID)
        {
            return info.id_ID;
        }
        else if (key == LanguageList.th_TH)
        {
            return info.th_TH;
        }
        
        return "";           
    }
    public void setCurLanguage(string curlanguage)
    {
        if (!this.isLanguageContains(curlanguage))
        {
            this._curLanguageStr = this._defLanguage;
        }
        else
        {
            this._curLanguageStr = curlanguage;
        }
    }

    public string getText(string key) {
        if (this._configDic == null || !this._configDic.ContainsKey(key)){
            return key;
        }
        else {
            return this.getText(this._configDic[key], this._curLanguageStr);
        }
    }

    public Dictionary<string, string> getLanguageMap() {
        if (this._languageMap == null) {
            this._languageMap = new Dictionary<string, string>();
            this._languageMap.Add(LanguageList.zh_CN, "简体中文");
            this._languageMap.Add(LanguageList.zh_TW, "繁体中文");
            this._languageMap.Add(LanguageList.en_US, "English");
        }
        return this._languageMap;
    }
}
