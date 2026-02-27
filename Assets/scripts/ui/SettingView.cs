using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SettingView : MonoBehaviour, IBaseView
{
    public GameObject language;
    public GameObject appearance;
    public GameObject resolution;
    public GameObject mainVolume;
    public GameObject musicVolume;
    public GameObject soundVolume;
    private string _curLanguage = "";
    private List<string> _keyList = null;
    public void init()
    {
         this._curLanguage = SettingDataMgr.Instance.getLanguage();
         this._keyList = LangMgr.Instance.getLanguageMap().Keys.ToList();
    }

    public void beforeShow()
    {

    }

    public void refresh()
    {

    }

    public void afterShow()
    {
        this.settingUI();
        this.setLanguageText();
    }

    private void settingUI() {
        this.mainVolume.GetComponent<Scrollbar>().value = SettingDataMgr.Instance.getMainVolume();
    }

    private void setLanguageText() {
        this.language.GetComponent<Text>().text = LangMgr.Instance.getLanguageMap()[this._curLanguage];
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onSwicthLanguage(int addValue) {
        int curIndex = 0;
        for (int i = 0; i < _keyList.Count; i++)
        {
            if (_keyList[i] == _curLanguage)
            {
                curIndex = i;
                break;
            }
        }
        curIndex += addValue;

        if (curIndex < 0)
        {
            curIndex = this._keyList.Count() - 1;
        }else if (curIndex >= this._keyList.Count()) { 
            curIndex = 0;
        }

        this._curLanguage = this._keyList[curIndex];
        this.setLanguageText();
    }

    public void onSwicthAppearance(int addValue) { 

    }

    public void onSelectResolution() { 

    }


    public void onMainVolumeClick() {
        float value = this.mainVolume.GetComponent<Scrollbar>().value;
        SettingDataMgr.Instance.setMainVolume(value);
    }

    public void onMusicVolumeClick() { 

    }

    public void onSoundVolumeClick() { 

    }

    public void onReturnClick()
    {
        if (SettingDataMgr.Instance.isSaveSetting()){
            UIMgr.Instance.showAlert("AlertView", "部分设置有改动，是否放弃更改并返回",
            () =>
            {
                UIMgr.Instance.closeView("SettingView");
            },
            () =>
            {
                
            });
        }
        else {
            UIMgr.Instance.closeView("SettingView");
        }
    }

    public void onDefualtClick() {
        SettingDataMgr.Instance.resetSetting();
        GamePropertyMgr.Instance.save();

        this._curLanguage = SettingDataMgr.Instance.getLanguage();
        LangMgr.Instance.setCurLanguage(this._curLanguage);
        UIMgr.Instance.closeView("SettingView");
        UIMgr.Instance.refreshView();
    }

    public void onSaveClick() {
        SettingDataMgr.Instance.setLanguage(this._curLanguage);
        SettingDataMgr.Instance.saveSetting();
        GamePropertyMgr.Instance.save();

        LangMgr.Instance.setCurLanguage(this._curLanguage);
        UIMgr.Instance.closeView("SettingView");
        UIMgr.Instance.refreshView();
    }
}
